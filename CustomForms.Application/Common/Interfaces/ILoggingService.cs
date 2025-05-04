namespace CustomForms.Application.Common.Interfaces;

public interface ILoggingService
{
	void LogInfo(string message);
	void LogWarn(string message);
	void LogDebug(string message);
	void LogError(string message);
}
