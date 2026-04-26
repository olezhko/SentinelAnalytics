namespace SentinelAnalytics.MAUI.Dto;

/// <summary>
/// Defines severity levels for categorizing the importance of messages or events, such as those used in logging or
/// error reporting.
/// </summary>
/// <remarks>Use the Severity enumeration to indicate the criticality of issues encountered during application
/// execution. This can help filter, prioritize, or respond to events based on their severity level.
/// Critical: Represents the most severe level, indicating a critical failure that may cause the application to crash or become unusable.
/// Error: Represents a issue that has occurred but may not necessarily cause the application to crash. It indicates a problem that should be addressed but may not be immediately critical.
/// Warning: Represents a potential issue or situation that may require attention but does not indicate an immediate problem. It serves as a cautionary level to highlight potential risks or areas for improvement.
/// </remarks>
public enum Severity { Critical, Error, Warning }