using System.Reflection.Metadata.Ecma335;
using PhysicsDemo.Data.GameData;

public class GameStepModel
{
    public Guid ID { get; set; }
    public int Time { get; set; }
    public int Force { get; set; }
    public double Acceleration { get; set; }
    public List<double> DoubleAcc => [Acceleration, Acceleration];
    public double Velocity { get; set; }
    public double Position { get; set; }
    public GameStepModel(GameStepModel model, int force, double mass, int t)
    {
        Time = model.Time + t;
        Force = force;
        Acceleration = force / mass;
        Velocity = model.Velocity + Acceleration * t;
        Position = model.Position + model.Velocity * t + Acceleration * 0.5 * t * t;
    }
    public GameStepModel()
    {
        Time = 0;
        Force = 0;
        Acceleration = 0;
        Velocity = 0;
        Position = 0;
    }
    public GameStepModel(GameStepModel model, PhysicsTurn turn)
    {
        Time = turn.RoundNumber;
        Acceleration = turn.TurnValue;
        Force = (int)Acceleration;
        Velocity = model.Velocity + Acceleration;
        Position = model.Position + model.Velocity + Acceleration * 0.5;
    }
}
