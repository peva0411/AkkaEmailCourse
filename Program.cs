using System;
﻿using Akka.Actor;
using WinTail.Actors;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActorSystem");

           
            // time to make your first actors!
          Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
          IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

          var tailCoordinatorActor = MyActorSystem.ActorOf(Props.Create<TailCoordinatorActor>(), "tailCoordinatorActor");


          Props validationActorProps = Props.Create(() => new FileValidationActor(consoleWriterActor, tailCoordinatorActor));
          IActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "validationActor");

          Props consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
          IActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);
            

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.AwaitTermination();
        }

      
    }
    #endregion
}
