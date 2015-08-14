using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail.Actors
{
  public class FileValidationActor : UntypedActor
  {
    private readonly IActorRef _consoleWriterActor;
    private readonly IActorRef _tailCoordinatorRef;

    public FileValidationActor(IActorRef consoleWriterActor, IActorRef tailCoordinatoRef)
    {
      _consoleWriterActor = consoleWriterActor;
      _tailCoordinatorRef = tailCoordinatoRef;
    }

    protected override void OnReceive(object message)
    {
      var msg = message as string;
      if (string.IsNullOrEmpty(msg))
      {
        // signal that the user needs to supply an input, as previously
        // received input was blank
        _consoleWriterActor.Tell(new Messages.NullInputError("No input received."));
      }
      else
      {
        var valid = IsValid(msg);
        if (valid)
        {
          _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));

          // continue reading messages from console
        }
        else
        {
          _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
        }
      }

      Sender.Tell(new Messages.ContinueProcessing());
    }

    private static bool IsValid(string msg)
    {
      var valid = msg.Length % 2 == 0;
      return valid;
    }
  }
}
