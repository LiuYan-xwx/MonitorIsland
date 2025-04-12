using ClassIsland.Core.Abstractions.Services;
using Microsoft.Extensions.Logging;
using MonitorIsland.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorIsland.Services;

public class MonitorService
{
    public ILogger<MonitorService> Logger { get; }
    public ILessonsService LessonsService { get; }
    private readonly MonitorHelper _monitorHelper;
    public MonitorService(ILogger<MonitorService> logger, ILessonsService lessonsService)
    {
        LessonsService = lessonsService;
        Logger = logger;
        LessonsService.PostMainTimerTicked += LessonsServiceOnPostMainTimerTicked;
        _monitorHelper = new MonitorHelper();
    }
    private async void LessonsServiceOnPostMainTimerTicked(object? sender, EventArgs e)
    {

    }
}
