using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail.Actors
{
  public class FileValidationActor : UntypedActor
  {
    private readonly IActorRef _consoleWriterActor;

    public FileValidationActor(IActorRef consoleWriterActor)
    {
      _consoleWriterActor = consoleWriterActor;
    }

    protected override void OnReceive(object message)
    {
      var msg = message as string;
      if (string.IsNullOrEmpty(msg))
      {
        // signal that the user needs to supply an input, as previously
        // received input was blank
        _consoleWriterActor.Tell(new Messages.NullInputError("No input received."));
        Sender.Tell(new Messages.ContinueProcessing());
      }
      else
      {
        var valid = IsFileUri(msg);
        if (valid)
        {
          _consoleWriterActor.Tell(new Messages.InputSuccess(string.Format("Starting processing for {0}",msg)));

          // start coordinator
          Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor")
            .Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));

        }
        else
        {
          //signal that input was bad
          _consoleWriterActor.Tell(new Messages.ValidationError(string.Format("{0} is not an existing URI on disk", msg)));

          // tell sender to continue doing its thing 
          Sender.Tell(new Messages.ContinueProcessing());
        }
      }

      Sender.Tell(new Messages.ContinueProcessing());
    }

    private bool IsFileUri(string path)
    {
      return File.Exists(path);
    }

    private static bool IsValid(string msg)
    {
      var valid = msg.Length % 2 == 0;
      return valid;
    }
  }
}
