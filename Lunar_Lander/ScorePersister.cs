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
    internal class ScorePersister
    {
        private bool saving = false;
        private bool loading = false;
        private string m_fileName;
        private Score[] m_highScores;
        

        public ScorePersister(string fileName) 
        {
            m_fileName = fileName;
            m_highScores = null;
        }

        public void Save(List<Score> scores)
        {
            lock (this)
            {
                saving = true;

                FinalizeSaveAsync(scores.ToArray());
            }
        }

        private async Task FinalizeSaveAsync(Score[] scores)
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
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(Score[]));
                                mySerializer.WriteObject(fs, scores);
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
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(Score[]));
                                    m_highScores = (Score[])mySerializer.ReadObject(fs);
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

        public List<Score> getHighScores()
        {
            if (m_highScores == null)
            {
                return null;
            }
            return new List<Score>(m_highScores);
        }
    }
}
