
public interface IMovingEntity{


    float CurrentSpeed { get; }

    void SetFreeze(bool value);
    void Move(float steering, float accel, float footbrake, float handbrake);
}
