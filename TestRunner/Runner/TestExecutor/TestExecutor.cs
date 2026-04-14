using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Exceptions;
using TestRunner.Models;

namespace TestRunner.Runner
{
    public class TestExecutor
    {
        private void ExecuteTestMethod(object? instance, MethodInfo method, object[]? parametres, CancellationToken cancellToken)
        {
            cancellToken.ThrowIfCancellationRequested();
            bool isAsync = method.ReturnType.IsAssignableFrom(typeof(Task))
                || method.ReturnType.IsGenericType && (method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>)
                                                      || method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
            Task? taskToWait = null;

            if (isAsync)
            {
                object? result = method.Invoke(instance, parametres);


                if (result is Task task)
                {
                    taskToWait = task;
                }
                else if (result is ValueTask valueTask)
                {
                    taskToWait = valueTask.AsTask();
                }
                taskToWait!.Wait(cancellToken);


            }
            else
            {
                taskToWait = Task.Run(() => method.Invoke(instance, parametres));
                taskToWait!.Wait(cancellToken);
            }

        }




        public TestResult GetTestResult(object? instance, TestInfo testInfo, CancellationToken cancellToken)
        {
            TestResult result = new TestResult(testInfo, ResultStrings.SUCCESS);


            if (testInfo.BeforeEach.Any())
            {
                foreach (var beforeEach in testInfo.BeforeEach)
                {
                    ExecuteTestMethod(instance, beforeEach, null, CancellationToken.None);
                }
            }
            try
            {
                ExecuteTestMethod(instance, testInfo.MethodInfo, testInfo.getParametres(), cancellToken);
            }
            catch (OperationCanceledException oex)
            {
                result.Status = ResultStrings.TIMEOUT;
                result.Ex = oex;
            }
            catch (AggregateException aex)
            {
                if (aex.InnerException is OperationCanceledException)
                {
                    result.Status = ResultStrings.TIMEOUT;
                }
                else if (aex.InnerException is TargetInvocationException targetInvocationException)
                {
                    if (targetInvocationException.InnerException is TestAssertionException)
                    {
                        result.Status = ResultStrings.FAILED;
                    }
                }
                else
                {
                    result.Status = ResultStrings.ERROR;
                }
                result.Ex = aex.InnerException;
            }
            catch (Exception e)
            {
                result.Status = ResultStrings.ERROR;
                result.Ex = e;
            }
            if (testInfo.AfterEach.Any())
            {
                foreach (var afterEach in testInfo.AfterEach)
                {
                    ExecuteTestMethod(instance, afterEach, null, CancellationToken.None);
                }
            }
            return result;
        }
    }
}
