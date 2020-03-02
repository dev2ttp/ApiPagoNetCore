using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebAPINetCore.Models
{
    public class apagarPC
    {
        string argumento = null;
        DateTime tmp;
        public apagarPC(string argumento, DateTime tmp)
        {
            this.argumento = argumento;
            this.tmp = tmp;
        }
        public bool Shut_Down()
        {
            try
            {
                while (true)
                {
                    if (tmp.ToLongTimeString() == DateTime.Now.ToLongTimeString())
                    {
                        Process proceso = new Process();
                        proceso.StartInfo.UseShellExecute = false;
                        proceso.StartInfo.RedirectStandardOutput = true;
                        proceso.StartInfo.FileName = "shutdown.exe";
                        proceso.StartInfo.Arguments = this.argumento;
                        proceso.Start();
                        break;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
