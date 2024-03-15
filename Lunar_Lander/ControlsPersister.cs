using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    internal class ControlsPersister
    {
        private bool saving = false;
        private bool loading = false;
        private string m_fileName;
        private Controls m_controls;


        public ControlsPersister(string fileName)
        {
            m_fileName = fileName;
            m_controls = null;
        }

        public void Save(Controls controls)
        {
            lock (this)
            {
                saving = true;

                FinalizeSaveAsync(controls);
            }
        }

        private async Task FinalizeSaveAsync(Controls controls)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile(m_fileName, FileMode.Create))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(Controls));
                                mySerializer.WriteObject(fs, controls);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Console.WriteLine("Save failed");
                    }
                }

                this.saving = false;
            });
        }

        public void Load()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    var result = FinalizeLoadAsync();
                    result.Wait();
                }
            }
        }

        private async Task FinalizeLoadAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists(m_fileName))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile(m_fileName, FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(Controls));
                                    m_controls = (Controls)mySerializer.ReadObject(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Console.WriteLine("Load failed");
                    }
                }

                this.loading = false;
            });
        }

        public Controls getControls()
        {
            if (m_controls == null)
            {
                return null;
            }
            return m_controls;
        }
    }
}
