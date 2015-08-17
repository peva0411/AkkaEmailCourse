using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using WinTail.Actors;

namespace WinTail
{
  public class FileObserver :IDisposable
  {
    private readonly IActorRef _tailActor;
    private readonly string _absoluteFilePath;
    private string _fileDir;
    private string _fileNameOnly;
    private FileSystemWatcher _watcher;

    public FileObserver(IActorRef tailActor, string absoluteFilePath)
    {
      _tailActor = tailActor;
      _absoluteFilePath = absoluteFilePath;
      _fileDir = 
        Path.GetDirectoryName(absoluteFilePath);
      _fileNameOnly = Path.GetFileName(absoluteFilePath);
    }

    public void Start()
    {
      _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly);

      _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

      _watcher.Changed += OnFileChanged;
      _watcher.Error += OnFileError;

      _watcher.EnableRaisingEvents = true;
    }

    private void OnFileError(object sender, ErrorEventArgs e)
    {
      _tailActor.Tell(new TailActor.FileError(_fileNameOnly, e.GetException().Message), ActorRefs.NoSender);
    }

    void OnFileChanged(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType == WatcherChangeTypes.Changed)
      {
        // here we use a special ActorRefs.NoSender
        // since this event can happen many times, this is a little microoptimization
        _tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
      }

    }

    public void Dispose()
    {
      _watcher.Dispose();
    }
  }
}
