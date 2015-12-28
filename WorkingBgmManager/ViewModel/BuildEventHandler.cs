using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace WorkingBgmManager.ViewModel
{
    class BuildEventHandler
    {
        private DTE dte;
        private Events dteEvents;
        private DebuggerEvents debuggerEvents;
        private Subject<Unit> onEnterRunModeSubject = new Subject<Unit>();
        private Subject<Unit> onEnterDesignModeSubject = new Subject<Unit>();

        public IObservable<Unit> OnEnterRunMode => onEnterRunModeSubject;
        public IObservable<Unit> OnEnterDesignMode => onEnterDesignModeSubject;

        public void Initialize()
        {
            dte = (DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE));
            dteEvents = dte.Events;
            debuggerEvents = dteEvents.DebuggerEvents;
            debuggerEvents.OnEnterRunMode += reason => onEnterRunModeSubject.OnNext(Unit.Default);
            debuggerEvents.OnEnterDesignMode += reason => onEnterDesignModeSubject.OnNext(Unit.Default);
        }
    }
}
