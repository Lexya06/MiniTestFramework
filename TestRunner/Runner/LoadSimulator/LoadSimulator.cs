using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRunner.Models;
using TestRunner.ThreadPool;

namespace TestRunner.Runner
{
    public class LoadSimulator
    {
        private readonly Runner _runner;
        private readonly TestExecutor _executor;
        private readonly Logger _logger;

        public LoadSimulator(Runner runner, TestExecutor executor, Logger logger)
        {
            _runner = runner;
            _executor = executor;
            _logger = logger;
        }

        public long RunUnevenLoadScenario(List<ClassInfo> availableClasses)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _logger.LogEvent("--- СТАРТ СИМУЛЯЦИИ НЕРАВНОМЕРНОЙ НАГРУЗКИ ---");

            if (availableClasses == null || availableClasses.Count == 0)
            {
                _logger.LogError("Симуляция прервана: список тестов пуст.");
                stopwatch.Stop();
                return stopwatch.ElapsedMilliseconds;
            }


            // 1. Единичные подачи
            _logger.LogEvent("> Сценарий 1: Единичные подачи");
            for (int i = 0; i < availableClasses.Count; i++)
            {
                var singleTestBatch = new List<ClassInfo> { availableClasses[i] };
                _runner.RunTests(singleTestBatch, _executor);
                Thread.Sleep(100);
            }

            // 2. Интервал бездействия
            _logger.LogEvent("> Сценарий 2: Интервал бездействия");
            Thread.Sleep(200);

            // 3. Пиковая нагрузка
            _logger.LogEvent("> Сценарий 3: Пиковая нагрузка (всплеск задач)");
           
            _runner.RunTests(availableClasses, _executor);
            


            _logger.LogEvent("--- СИМУЛЯЦИЯ ЗАВЕРШЕНА ---");
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        
    }
}
