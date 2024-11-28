using Microsoft.Extensions.Logging;
using System;

namespace S7Svr.Simulator.ViewModels
{
    public record LogMessage(DateTime Timestamp, LogLevel Level, string Content);
}
