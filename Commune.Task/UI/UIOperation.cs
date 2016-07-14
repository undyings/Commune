using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Commune.Basis;

namespace Commune.Task
{
  public class UIOperation : ICanceler
  {
    public readonly string OperationName;
    public readonly UITaskPull Pull;
    //public volatile float TotalProgressInterval;

    public UIOperation(UITaskPull pull, string operationName) :
      this(pull, operationName, 1)
    {
    }

    public readonly OperationProgress Progress;
    public UIOperation(UITaskPull pull, string operationName, int stepCount)
    {
      this.Pull = pull;
      this.OperationName = operationName;
      this.Progress = new OperationProgress(this, stepCount);
    }

    public void SetAndStartBackgroundTask(IEnumerable<Step> backgroundSteps)
    {
      this.BackgroundTask = new Task(backgroundSteps, this, delegate { ForceUI(); });
      Pull.StartBackgroundTask(BackgroundTask);
    }

    static IEnumerable<Step> SimpleTask(Getter<object> backgroundAction)
    {
      object result = backgroundAction();
      yield return new FinishStep(result);
    }

    public void SetAndStartBackgroundTask(Getter<object> backgroundAction)
    {
      SetAndStartBackgroundTask(SimpleTask(backgroundAction));
    }

    public void SetAndStartUITask(IEnumerable<Step> uiSteps)
    {
      this.UITask = new Task(uiSteps);
      Pull.StartUITask(UITask);
    }

    public volatile Task BackgroundTask = null;
    public volatile Task UITask = null;

    public void ForceUI()
    {
      Task task = UITask;
      if (task != null)
        task.Force();
    }

    volatile bool cancelation = false;
    public bool Cancelation
    {
      get { return cancelation; }
    }

    public void Cancel()
    {
      cancelation = true;
      ForceUI();
    }

    //volatile int stepCount;
    //public void SetStepCount(int stepCount)
    //{
    //  this.stepCount = stepCount;
    //}

    //volatile int currentStep = 0;
    //volatile float currentStepSize = 1;
    //volatile float currentStepProgress = 0;

    //public float CurrentStepProgress
    //{
    //  get { return currentStepProgress; }
    //  set
    //  {
    //    currentStepProgress = value;
    //    ForceUI();
    //  }
    //}

    //public void NextStep()
    //{
    //  NextStep(1);
    //}

    //public void NextStep(float stepSize)
    //{
    //  currentStep++;
    //  currentStepSize = stepSize;
    //  currentStepProgress = 0;

    //  ForceUI();
    //}

    //public int ProgressPercent
    //{
    //  get
    //  {
    //    int count = Math.Max(1, stepCount);
    //    float stepSize = Math.Max(1, currentStepSize);

    //    return (int)((Math.Min(count, Math.Max(0, currentStep - 1)) +
    //      Math.Min(1, currentStepProgress / stepSize)) * 100 / count);
    //  }
    //}

    public class OperationProgress
    {
      readonly object lockObj = new object();

      public volatile string CurrentStepText = "";
      readonly Queue<string> stepTextQueue = new Queue<string>();
      public void EnqueueStepText(string stepText)
      {
        lock (lockObj)
          stepTextQueue.Enqueue(stepText);

        operation.ForceUI();
      }
      public string[] DequeueStepText()
      {
        lock (lockObj)
        {
          string[] steps = new string[stepTextQueue.Count];
          for (int i = 0; i < steps.Length; ++i)
          {
            steps[i] = stepTextQueue.Dequeue();
          }
          return steps;
        }
      }

      volatile int stepCount;
      volatile int currentStep = -1;
      volatile float currentStepSize;
      volatile float currentStepProgress;

      public void SetStepCount(int stepCount)
      {
        this.stepCount = stepCount;
      }

      readonly UIOperation operation;
      internal OperationProgress(UIOperation operation, int stepCount)
      {
        this.operation = operation;
        this.stepCount = stepCount;
      }

      public void BeginStep()
      {
        BeginStep(1);
      }

      public void BeginStep(float stepSize)
      {
        currentStep++;
        currentStepSize = stepSize;
        currentStepProgress = 0;

        //operation.ForceUI();
      }

      public float StepProgress
      {
        get { return currentStepProgress; }
        set
        {
          currentStepProgress = value;

          //operation.ForceUI();
        }
      }

      public int Percent
      {
        get
        {
          int count = Math.Max(1, stepCount);

          if (currentStep >= count)
            return 100;

          int stepIndex = Math.Max(0, currentStep);
          float stepSize = Math.Max(1, currentStepSize);
          float stepProgress = Math.Max(0, Math.Min(stepSize, currentStepProgress));

          return (int)((stepIndex + stepProgress / stepSize) * 100 / count);
        }
      }
    }
  }
}
