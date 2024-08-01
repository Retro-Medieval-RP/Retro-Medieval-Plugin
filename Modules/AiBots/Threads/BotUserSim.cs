using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using SDG.Framework.Water;
using SDG.Unturned;
using UnityEngine;

namespace AiBots.Threads;

public class BotUserSim
{
    private const float CSwim = 3f;
    private const float CJump = 7f;
    private const float CMinAngleSit = 60f;
    private const float CMaxAngleSit = 120f;
    private const float CMinAngleClimb = 45f;
    private const float CMaxAngleClimb = 100f;
    private const float CMinAngleSwim = 45f;
    private const float CMaxAngleSwim = 135f;
    private const float CMinAngleStand = 0.0f;
    private const float CMaxAngleStand = 180f;
    private const float CMinAngleCrouch = 20f;
    private const float CMaxAngleCrouch = 160f;
    private const float CMinAngleProne = 60f;
    private const float CMaxAngleProne = 120f;

    private static readonly
#nullable disable
        FieldInfo SServerSidePacketsField =
            typeof(PlayerInput).GetField("serversidePackets", BindingFlags.Instance | BindingFlags.NonPublic);

    private readonly ushort[] _mFlags;
    private readonly bool[] _mKeys;
    private readonly List<PlayerInputPacket> _mPlayerInputPackets;
    private readonly Player _player;
    private Vector3 _mVelocity;
    private uint _mCount;
    private uint _mBuffer;
    private uint _mConsumed;
    private uint _mSimulation;
    private float _mYaw;
    private float _mPitch;
    private float _mTimeLerp;

    private static readonly PropertyInfo SIsBoostingProperty =
        typeof(InteractableVehicle).GetProperty("isBoosting", BindingFlags.Instance | BindingFlags.Public);

    private static readonly FieldInfo SSpeedField =
        typeof(InteractableVehicle).GetField("_speed", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo SPhysicsSpeedField =
        typeof(InteractableVehicle).GetField("_physicsSpeed", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo SFactorField =
        typeof(InteractableVehicle).GetField("_factor", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo SBuoyancyField =
        typeof(InteractableVehicle).GetField("buoyancy", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo SSpeedTractionField =
        typeof(InteractableVehicle).GetField("speedTraction", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo SLastUpdatedPosField =
        typeof(InteractableVehicle).GetField("lastUpdatedPos", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo SIsPhysicalField =
        typeof(InteractableVehicle).GetField("isPhysical", BindingFlags.Instance | BindingFlags.NonPublic);

    private float _mFactor;

    private
#nullable enable
        Rigidbody? _mRigidbody;

    private Transform? _mBuoyancy;
    private float _mAltSpeedInput;
    private float _mAltSpeedOutput;
    private float _mSpeedTraction;

    public bool Enabled { get; set; }

    public Vector3 Move { get; set; }

    public bool Jump
    {
        get => _mKeys[0];
        set => _mKeys[0] = value;
    }

    public MouseState MouseState
    {
        get => (MouseState)((_mKeys[1] ? 1 : 0) + (_mKeys[2] ? 2 : 0));
        set
        {
            if (value > MouseState.LeftRight || value < MouseState.None)
                throw new ArgumentOutOfRangeException(nameof(MouseState));
            var flag1 = (value & MouseState.Left) != 0;
            var flag2 = (value & MouseState.Right) != 0;
            _mKeys[1] = flag1;
            _mKeys[2] = flag2;
        }
    }

    public bool Crouch
    {
        get => _mKeys[3];
        set => _mKeys[3] = value;
    }

    public bool Prone
    {
        get => _mKeys[4];
        set => _mKeys[4] = value;
    }

    public bool Sprint
    {
        get => _mKeys[5];
        set => _mKeys[5] = value;
    }

    public bool LeanLeft
    {
        get => _mKeys[6];
        set => _mKeys[6] = value;
    }

    public bool LeanRight
    {
        get => _mKeys[7];
        set => _mKeys[7] = value;
    }

    public bool PluginKey1
    {
        get => _mKeys[9];
        set => _mKeys[9] = value;
    }

    public bool PluginKey2
    {
        get => _mKeys[10];
        set => _mKeys[10] = value;
    }

    public bool PluginKey3
    {
        get => _mKeys[11];
        set => _mKeys[11] = value;
    }

    public bool PluginKey4
    {
        get => _mKeys[12];
        set => _mKeys[12] = value;
    }

    public bool PluginKey5
    {
        get => _mKeys[13];
        set => _mKeys[13] = value;
    }

    public BotUserSim(
#nullable disable
        Player player)
    {
        _player = player;
        _mCount = 0U;
        _mBuffer = 0U;
        _mConsumed = 0U;
        Move = Vector3.zero;
        _mPlayerInputPackets = new List<PlayerInputPacket>();
        _mKeys = new bool[9 + ControlsSettings.NUM_PLUGIN_KEYS];
        _mFlags = new ushort[9 + ControlsSettings.NUM_PLUGIN_KEYS];
        for (byte index = 0; index < 9 + ControlsSettings.NUM_PLUGIN_KEYS; ++index)
            _mFlags[index] = (ushort)(1U << index);
        StartHook();
    }

    public async void StartHook()
    {
        Enabled = true;
        await Start();
    }

    public void SetRotation(float yaw, float pitch, float time)
    {
        if (!yaw.IsFinite())
            yaw = 0.0f;
        if (!pitch.IsFinite())
            pitch = 0.0f;
        if (!time.IsFinite() || time <= 0.0)
            time = 1f;
        _mYaw = yaw;
        _mPitch = pitch;
        _mTimeLerp = time;
        ClampPitch();
        ClampYaw();
    }

    public async UniTask Start()
    {
        var uniTask = UniTask.DelayFrame(5, PlayerLoopTiming.FixedUpdate);
        await uniTask;
        var queue = (Queue<PlayerInputPacket>)SServerSidePacketsField.GetValue(_player.input);
        while (Enabled)
        {
            await UniTask.SwitchToMainThread();
            await UniTask.WaitForFixedUpdate();
            if (_mCount % PlayerInput.SAMPLES == 0U)
            {
                for (var i = 0; i < _mKeys.Length; ++i)
                    _player.input.keys[i] = _mKeys[i];
                var movement = _player.movement;
                var transform = _player.transform;
                var move = Move;
                var normalizedMove = move.normalized;
                var speed = movement.speed;
                var deltaTime = PlayerInput.RATE;
                var stance = _player.stance.stance;
                var controller = movement.controller;
                var aim = _player.look.aim;
                if ((int)stance <= 1)
                {
                    if (stance != null)
                    {
                        if (stance == (EPlayerStance)1)
                        {
                            if (_player.stance.isSubmerged ||
                                _player.look.pitch > 110.0 && Move.z > 0.10000000149011612)
                            {
                                _mVelocity = aim.rotation * normalizedMove * speed * 1.5f;
                                if (Jump)
                                    _mVelocity.y = 3f * movement.pluginJumpMultiplier;
                                controller.CheckedMove(_mVelocity * deltaTime);
                                goto label_41;
                            }
                            else
                            {
                                WaterUtility.getUnderwaterInfo(transform.position, out var flag,
                                    out var surfaceElevation);
                                var rotation = transform.rotation;
                                move = Move;
                                var normalized = move.normalized;
                                _mVelocity = rotation * normalized * speed * 1.5f;
                                _mVelocity.y = (float)((surfaceElevation - 1.2749999761581421 - transform.position.y) /
                                                       8.0);
                                controller.CheckedMove(_mVelocity * deltaTime);
                                goto label_41;
                            }
                        }
                    }
                    else
                    {
                        _mVelocity = new Vector3(0.0f, (float)(Move.z * (double)speed * 0.5), 0.0f);
                        controller.CheckedMove(_mVelocity * deltaTime);
                        goto label_41;
                    }
                }
                else if (stance != (EPlayerStance)6)
                {
                    if (stance == (EPlayerStance)7)
                        goto label_41;
                }
                else
                {
                    uniTask = SimulateVehicle();
                    await uniTask;
                    goto label_41;
                }

                var isMovementBlocked = false;
                var shouldUpdateVelocity = false;
                if (movement.isGrounded && movement.ground.normal.y > 0.0)
                {
                    var slopeAngle = Vector3.Angle(Vector3.up, movement.ground.normal);
                    var maxWalkableSlope = 59f;
                    var info = Level.info;
                    int num1;
                    if (info == null)
                    {
                        num1 = 0;
                    }
                    else
                    {
                        var maxWalkableSlope1 = info.configData?.Max_Walkable_Slope;
                        var num2 = -0.5f;
                        num1 = maxWalkableSlope1.GetValueOrDefault() > (double)num2 & maxWalkableSlope1.HasValue
                            ? 1
                            : 0;
                    }

                    if (num1 != 0)
                        maxWalkableSlope = Level.info.configData.Max_Walkable_Slope;
                    if (slopeAngle > (double)maxWalkableSlope)
                    {
                        isMovementBlocked = true;
                        var a = Vector3.Cross(Vector3.Cross(Vector3.up, movement.ground.normal),
                            movement.ground.normal);
                        _mVelocity += a * 16f * PlayerInput.RATE;
                        shouldUpdateVelocity = true;
                        a = new Vector3();
                    }
                }

                if (!isMovementBlocked)
                {
                    var moveVector = movement.transform.rotation * normalizedMove * speed;
                    if (movement.isGrounded)
                    {
                        moveVector = Vector3.Cross(Vector3.Cross(Vector3.up, moveVector), movement.ground.normal);
                        moveVector.y = Mathf.Min(moveVector.y, 0.0f);
                        _mVelocity = moveVector;
                    }
                    else
                    {
                        _mVelocity.y += (float)(Physics.gravity.y *
                                                (movement.fall <= 0.0 ? movement.totalGravityMultiplier : 1.0) *
                                                deltaTime * 3.0);
                        var maxFall = movement.totalGravityMultiplier < 0.99000000953674316
                            ? Physics.gravity.y * 2f * movement.totalGravityMultiplier
                            : -100f;
                        _mVelocity.y = Mathf.Max(maxFall, _mVelocity.y);
                        var horizontalMagnitude = moveVector.GetHorizontalMagnitude();
                        var horizontal = _mVelocity.GetHorizontal();
                        var horizontalMagnitude2 = _mVelocity.GetHorizontalMagnitude();
                        float maxMagnitude;
                        if (horizontalMagnitude2 > (double)horizontalMagnitude)
                        {
                            var num5 = 2f * Provider.modeConfigData.Gameplay.AirStrafing_Deceleration_Multiplier;
                            maxMagnitude = Mathf.Max(horizontalMagnitude, horizontalMagnitude2 - num5 * deltaTime);
                        }
                        else
                            maxMagnitude = horizontalMagnitude;

                        var a3 = moveVector * 4f * Provider.modeConfigData.Gameplay.AirStrafing_Acceleration_Multiplier;
                        var vector2 = horizontal + a3 * deltaTime;
                        vector2 = vector2.ClampHorizontalMagnitude(maxMagnitude);
                        _mVelocity.x = vector2.x;
                        _mVelocity.z = vector2.z;
                        shouldUpdateVelocity = true;
                    }
                }

                var jumpMastery = _player.skills.mastery(0, 6);
                if (Jump && movement.isGrounded && !_player.life.isBroken &&
                    _player.life.stamina >= 10.0 * (1.0 - jumpMastery * 0.5) &&
                    (stance == (EPlayerStance)3 || stance == (EPlayerStance)2))
                {
                    _mVelocity.y = (float)(7.0 * (1.0 + jumpMastery * 0.25)) * movement.pluginJumpMultiplier;
                    _player.life.askTire((byte)(10.0 * (1.0 - jumpMastery * 0.5)));
                }

                _mVelocity += movement.pendingLaunchVelocity;
                movement.pendingLaunchVelocity = Vector3.zero;
                var previousPosition = movement.transform.position;
                movement.controller.CheckedMove(_mVelocity * PlayerInput.RATE);
                if (shouldUpdateVelocity)
                    _mVelocity = (movement.transform.position - previousPosition) / PlayerInput.RATE;
                label_41:
                PlayerInputPacket packet;
                if (_player.stance.stance == (EPlayerStance)6)
                {
                    _mPlayerInputPackets.Add(packet = new DrivingPlayerInputPacket());
                }
                else
                {
                    _mPlayerInputPackets.Add(packet = new WalkingPlayerInputPacket());
                }

                packet.clientSimulationFrameNumber = _mSimulation;
                ++_mSimulation;
                _mBuffer += PlayerInput.SAMPLES;
            }

            if (_mConsumed < _mBuffer)
                ++_mConsumed;
            if ((int)_mConsumed == (int)_mBuffer && _mPlayerInputPackets.Count > 0)
            {
                ushort compressedKeys = 0;
                for (var b = 0; b < _mKeys.Length; ++b)
                {
                    if (_mKeys[b])
                        compressedKeys |= _mFlags[b];
                }

                var playerInputPacket2 = _mPlayerInputPackets[_mPlayerInputPackets.Count - 1];
                playerInputPacket2.keys = compressedKeys;
                switch (playerInputPacket2)
                {
                    case DrivingPlayerInputPacket drivingPlayerInputPacket:
                        var vehicle = _player.movement.getVehicle();
                        if (vehicle != null)
                        {
                            var transform = vehicle.transform;
                            drivingPlayerInputPacket.position = vehicle.asset.engine == (EEngine)5
                                ? new Vector3(vehicle.roadPosition, 0.0f, 0.0f)
                                : vehicle.transform.position;
                            drivingPlayerInputPacket.rotation = transform.rotation;
                            drivingPlayerInputPacket.speed = (byte)(Mathf.Clamp(vehicle.speed, -100f, 100f) + 128.0);
                            drivingPlayerInputPacket.physicsSpeed =
                                (byte)(Mathf.Clamp(vehicle.physicsSpeed, -100f, 100f) + 128.0);
                            drivingPlayerInputPacket.turn = (byte)(Move.x + 1.0);
                        }

                        break;
                    case WalkingPlayerInputPacket walkingPlayerInputPacket:
                        var horizontal = (byte)(Move.x + 1.0);
                        var vertical = (byte)(Move.y + 1.0);
                        walkingPlayerInputPacket.analog = (byte)((uint)horizontal << 4 | vertical);
                        walkingPlayerInputPacket.clientPosition = _player.transform.position;
                        walkingPlayerInputPacket.yaw = Mathf.Lerp(_player.look.yaw, _mYaw, _mTimeLerp);
                        walkingPlayerInputPacket.pitch = Mathf.Lerp(_player.look.pitch, _mPitch, _mTimeLerp);
                        break;
                }

                foreach (var playerInputPacket1 in _mPlayerInputPackets)
                    queue.Enqueue(playerInputPacket1);
                _mPlayerInputPackets.Clear();
            }

            ++_mCount;
        }
    }

    private void ClampPitch()
    {
        var vehicleSeat = _player.movement.getVehicleSeat();
        var num1 = 0.0f;
        var num2 = 180f;
        if (vehicleSeat != null)
        {
            if (vehicleSeat.turret != null)
            {
                num1 = vehicleSeat.turret.pitchMin;
                num2 = vehicleSeat.turret.pitchMax;
            }
            else
            {
                num1 = 60f;
                num2 = 120f;
            }
        }
        else
        {
            switch ((int)_player.stance.stance)
            {
                case 0:
                    num1 = 45f;
                    num2 = 100f;
                    break;
                case 1:
                    num1 = 45f;
                    num2 = 135f;
                    break;
                case 2:
                case 3:
                    num1 = 0.0f;
                    num2 = 180f;
                    break;
                case 4:
                    num1 = 20f;
                    num2 = 160f;
                    break;
                case 5:
                    num1 = 60f;
                    num2 = 120f;
                    break;
            }
        }

        _mPitch = Mathf.Clamp(_mPitch, num1, num2);
    }

    private void ClampYaw()
    {
        _mYaw %= 360f;
        var vehicleSeat = _player.movement.getVehicleSeat();
        if (vehicleSeat == null)
            return;
        var num1 = -90f;
        var num2 = 90f;
        if (vehicleSeat.turret != null)
        {
            num1 = vehicleSeat.turret.yawMin;
            num2 = vehicleSeat.turret.yawMax;
        }
        else if (_player.stance.stance == (EPlayerStance)6)
        {
            num1 = -160f;
            num2 = 160f;
        }

        _mYaw = Mathf.Clamp(_mYaw, num1, num2);
    }

    private async UniTask SimulateVehicle()
    {
        await UniTask.SwitchToMainThread();
        var vehicle = _player.movement.getVehicle();
        if (vehicle != null)
        {
            _mFactor = 0.0f;
            _mRigidbody = null;
            _mBuoyancy = null;
            _mAltSpeedInput = 0.0f;
            _mAltSpeedOutput = 0.0f;
            _mSpeedTraction = 0.0f;
        }
        else
        {
            var asset = vehicle.asset;
            _mFactor = (float)SFactorField.GetValue(vehicle);
            _mSpeedTraction = (float)SSpeedTractionField.GetValue(vehicle);
            if (!_mBuoyancy)
                _mBuoyancy = (Transform)SBuoyancyField.GetValue(vehicle);
            if (!_mRigidbody)
                _mRigidbody = vehicle.GetComponent<Rigidbody>();
            _mRigidbody!.useGravity = asset.engine != (EEngine)5;
            _mRigidbody.isKinematic = asset.engine == (EEngine)5;
            var wheelArray1 = vehicle.tires;
            foreach (var wheel in wheelArray1)
            {
                var tire = wheel;
                tire.isPhysical = true;
            }

            var moveY = Move.z;
            var speed = 1f;
            if (asset.useStaminaBoost)
            {
                int num1;
                if (Sprint)
                {
                    var stamina = vehicle.passengers[0]?.player?.player.life.stamina;
                    var nullable = stamina.HasValue ? stamina.GetValueOrDefault() : new int?();
                    const int num2 = 0;
                    num1 = nullable.GetValueOrDefault() > num2 & nullable.HasValue ? 1 : 0;
                }
                else
                    num1 = 0;

                if (num1 != 0)
                {
                    SIsBoostingProperty.SetValue(vehicle, true);
                }
                else
                {
                    SIsBoostingProperty.SetValue(vehicle, false);
                    moveY *= asset.staminaBoost;
                    speed *= asset.staminaBoost;
                }
            }
            else
                SIsBoostingProperty.SetValue(vehicle, false);

            SSpeedField.SetValue(vehicle, 150f);
            SIsPhysicalField.SetValue(vehicle, true);
            if (vehicle.usesFuel && vehicle.fuel == 0 || vehicle.isUnderwater || vehicle.isDead || !vehicle.isEngineOn)
            {
                moveY = 0.0f;
                speed = 1f;
            }

            _mFactor = Mathf.InverseLerp(0.0f, vehicle.speed < 0.0 ? asset.speedMin : asset.speedMax, vehicle.speed);
            var tireOnGround = false;
            if (vehicle.tires != null)
            {
                var wheelArray2 = vehicle.tires;

                foreach (var tire in wheelArray2)
                {
                    tire.simulate(Move.x, moveY, Jump, PlayerInput.RATE);
                    tire.update(Time.deltaTime);
                    tireOnGround |= tire.isGrounded;
                }
            }

            var engine1 = asset.engine;
            switch ((int)engine1)
            {
                case 0:
                    if (tireOnGround)
                    {
                        _mRigidbody.AddForce(-vehicle.transform.up * _mFactor * 40f);
                        _mRigidbody.AddForce(vehicle.transform.forward * 20f);
                    }

                    if (_mBuoyancy != null)
                    {
                        var lerpSteerCar = Mathf.Lerp(asset.airSteerMax, asset.airSteerMin, _mFactor);
                        var isUnderWater = WaterUtility.isPointUnderwater(vehicle.transform.position - Vector3.up);
                        _mSpeedTraction = Mathf.Lerp(_mSpeedTraction, isUnderWater ? 0.0f : 1f, 4f * Time.deltaTime);
                        if (!MathfEx.IsNearlyZero(_mSpeedTraction))
                        {
                            var num = moveY > 0.0
                                ? Mathf.Lerp(_mAltSpeedInput, asset.speedMax, PlayerInput.RATE / 4f)
                                : (moveY < 0.0
                                    ? Mathf.Lerp(_mAltSpeedInput, asset.speedMin, PlayerInput.RATE / 4f)
                                    : Mathf.Lerp(_mAltSpeedInput, 0.0f, PlayerInput.RATE / 8f));

                            _mAltSpeedInput = num;
                            _mAltSpeedOutput = _mAltSpeedInput * _mSpeedTraction;
                            var forward = vehicle.transform.forward;
                            forward.y = 0.0f;
                            _mRigidbody.AddForce(forward.normalized * _mAltSpeedOutput * 2f * _mSpeedTraction);
                            _mRigidbody.AddRelativeTorque(Move.z * -2.5f * _mSpeedTraction,
                                (float)(Move.x * (double)lerpSteerCar / 8.0) * _mSpeedTraction,
                                Move.x * -2.5f * _mSpeedTraction);
                        }
                    }

                    break;
                case 1:
                    var lerpSteer = Mathf.Lerp(asset.airSteerMax, asset.airSteerMin, _mFactor);
                    var num3 = lerpSteer > 0.0
                        ? Mathf.Lerp(_mAltSpeedInput, asset.speedMax * speed, PlayerInput.RATE)
                        : (lerpSteer < 0.0
                            ? Mathf.Lerp(_mAltSpeedInput, 0.0f, PlayerInput.RATE / 8f)
                            : Mathf.Lerp(_mAltSpeedInput, 0.0f, PlayerInput.RATE / 16f));
                    _mAltSpeedInput = num3;
                    _mAltSpeedOutput = _mAltSpeedInput;
                    _mRigidbody.AddForce(vehicle.transform.forward * _mAltSpeedOutput * 2f);
                    _mRigidbody.AddForce(Mathf.Lerp(0.0f, 1f,
                                             vehicle.transform.InverseTransformDirection(_mRigidbody.velocity).z /
                                             asset.speedMax) *
                                         asset.lift * -Physics.gravity);
                    if (vehicle.tires == null || vehicle.tires.Length == 0 ||
                        !vehicle.tires[0].isGrounded && !vehicle.tires[1].isGrounded)
                        _mRigidbody.AddRelativeTorque(
                            Mathf.Clamp(Move.z, -asset.airTurnResponsiveness, asset.airTurnResponsiveness) * lerpSteer,
                            (float)(Move.x * (double)asset.airTurnResponsiveness * lerpSteer / 4.0),
                            (float)(Mathf.Clamp(Move.x, -asset.airTurnResponsiveness, asset.airTurnResponsiveness) *
                                -(double)lerpSteer / 2.0));
                    if (vehicle.tires == null || vehicle.tires.Length == 0 && moveY < 0.0)
                    {
                        _mRigidbody.AddForce(vehicle.transform.forward * asset.speedMin * 4f);
                    }

                    break;
            }

            var num4 = _mAltSpeedOutput;
            speed = num4;
            var engine3 = asset.engine;
            var num5 = engine3 == (EEngine)5
                ? _mAltSpeedOutput
                : vehicle.transform.InverseTransformDirection(_mRigidbody.velocity).z;
            SSpeedField.SetValue(vehicle, speed);
            SPhysicsSpeedField.SetValue(vehicle, num5);
            SFactorField.SetValue(vehicle, _mFactor);
            SSpeedTractionField.SetValue(vehicle, _mSpeedTraction);
            SLastUpdatedPosField.SetValue(vehicle, vehicle.transform.position);
        }
    }
}