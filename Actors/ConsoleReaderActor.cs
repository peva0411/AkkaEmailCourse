using System;
using Akka.Actor;

namespace WinTail.Actors
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Shutdown"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
        public static string StartCommand = "start";
     
        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }
           
            GetAndValidateInput();
        }

      private void GetAndValidateInput()
      {
        var message = Console.ReadLine();

        if (!string.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
        {
          // if user typed ExitCommand, shut down the entire actor system (allows the process to exit)
          Context.System.Shutdown();
          return;
        }

        // otherwise, just hand message off to validation actor (by telling its actor ref)
        Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell(message);
      }

        private void DoPrintInstructions()
        {
          Console.WriteLine("Please provide the URI of a log file on disk.\n");
        }
    }
}