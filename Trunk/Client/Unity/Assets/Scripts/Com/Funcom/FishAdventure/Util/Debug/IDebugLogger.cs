using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	interface IDebugLogger
	{
        void Log(String text);

        void Log(String text, Severity severity);

        void Log(String text, Severity severity, Exception e);
        
        void Log(String text, Exception e);

        void LogWarning(String text);

        void LogWarning(String text, Exception e);

        void LogError(String text);

        void LogError(String text, Exception e);

        Boolean IsExtreme();
	}
