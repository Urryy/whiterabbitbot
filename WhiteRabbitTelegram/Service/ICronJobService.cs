namespace WhiteRabbitTelegram.Service;

public interface ICronJobService
{
    void DoJob(string cronExpression);
}
