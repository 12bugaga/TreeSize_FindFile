using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TreeSize__FindFile.ModelView;

namespace TreeSize.ViewModel
{
    public class MyViewModel
    {
        protected ConcurrentStack<ViewItems> concurrentStack = new ConcurrentStack<ViewItems>();
        protected int changeCoutAll, changeCountCoincidences;
        DateTime startTime;
        
        ViewItems viewItems;
        Regex regex;
        CancellationTokenSource cts;
        ManualResetEvent manualResetEvent;
        public MyViewModel(ViewItems _viewItems, Regex _regex, CancellationTokenSource _cts, ManualResetEvent _manualResetEvent)
        {
            cts = _cts;
            viewItems = _viewItems;
            regex = _regex;
            manualResetEvent = _manualResetEvent;
        }

        private void GetTime(object obj)
        {
            
           if(!cts.Token.IsCancellationRequested)
            {
                manualResetEvent.WaitOne();
                viewItems.TimeCount = (DateTime.Now - startTime).Seconds.ToString();
            }
        }

        public async void GetItemFromPathAsync(string path)
        {
            await Task.Run(() => GetItemFromPath(path));
            manualResetEvent.WaitOne();
        }

        private void GetItemFromPath(string path)
        {
            startTime = DateTime.Now;
            TimerCallback tm = new TimerCallback(GetTime);
            Timer timer = new Timer(tm, null, 0, 500);
            
            ModelItem item;
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] diA = di.GetDirectories();
                FileInfo[] fi = di.GetFiles();
                double catalogSize;

                // Cycle for directory
                foreach (DirectoryInfo df in diA)
                {
                    manualResetEvent.WaitOne();

                    catalogSize = 0;
                    item = (new ModelItem
                    {
                        Header = GetFileOrFolderName(df.FullName),
                        FullPath = df.FullName,
                        Status = "Waiting...",
                        Image = GetImage(df.FullName)
                    });
                    viewItems.AddItem(item);

                    item = (new ModelItem
                    {
                        Header = GetFileOrFolderName(df.FullName),
                        FullPath = df.FullName,
                        Status = "",
                        Image = GetImage(df.FullName),
                        ModelItems = GetChildrenItem(df.FullName),
                        VolumeMemory = GetVolMem(df.FullName, ref catalogSize),
                    });
                    
                    if(item.ModelItems.Count !=0)
                    {
                        concurrentStack.Push(viewItems);
                        viewItems.ChangeItem(concurrentStack, item);
                    }
                    else
                    {
                        concurrentStack.Push(viewItems);
                        viewItems.Delete(concurrentStack, item);
                    }
                }

                // Cycle for all files
                foreach (FileInfo f in fi)
                {
                    manualResetEvent.WaitOne();

                    MatchCollection matches = regex.Matches(GetFileOrFolderName(f.FullName));
                    if (matches.Count > 0)
                    {
                        item = new ModelItem
                        {
                            Header = GetFileOrFolderName(f.FullName),
                            FullPath = f.FullName,
                            Status = null,
                            Image = GetImage(f.FullName),
                            VolumeMemory = f.Length,
                        };
                        viewItems.AddItem(item);
                        viewItems.CountCoincidencesFiles = Interlocked.Increment(ref changeCountCoincidences);
                    }
                    viewItems.CountAllFiles = Interlocked.Increment(ref changeCoutAll);
                }
            }

            catch
            {
                item = new ModelItem
                {
                    Header = GetFileOrFolderName(path),
                    FullPath = path,
                    Image = GetImage(path),
                    Status = "Not available",
                    VolumeMemory = -1
                };
                viewItems.AddItem(item);
            }
            cts.Cancel();
        }

        private ObservableCollection<ModelItem> GetChildrenItem(string path/*, CancellationToken token*/)
        {
            ObservableCollection<ModelItem> childrenItems = new ObservableCollection<ModelItem>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] diA = di.GetDirectories();
                FileInfo[] fi = di.GetFiles();
                double catalogSize;

                // Cycle for directory
                foreach (DirectoryInfo df in diA)
                {
                    manualResetEvent.WaitOne();

                    catalogSize = 0;
                    ModelItem item = new ModelItem
                    {
                        VolumeMemory = GetVolMem(df.FullName, ref catalogSize),
                        ModelItems = GetChildrenItem(df.FullName/*, token*/),
                        Status = null,
                        Header = GetFileOrFolderName(df.FullName),
                        FullPath = df.FullName,
                        Image = GetImage(df.FullName)
                    };
                    if(item.ModelItems.Count !=0)
                    {
                        childrenItems.Add(item);
                    }

                }
                // Cycle for all files
                foreach (FileInfo f in fi)
                {
                    manualResetEvent.WaitOne();

                    MatchCollection matches = regex.Matches(GetFileOrFolderName(f.FullName));
                    if (matches.Count > 0)
                    { 
                        childrenItems.Add(new ModelItem
                        {
                            Header = GetFileOrFolderName(f.FullName),
                            FullPath = f.FullName,
                            Status = null,
                            Image = GetImage(f.FullName),
                            VolumeMemory = f.Length,
                        });
                        viewItems.CountCoincidencesFiles = Interlocked.Increment(ref changeCountCoincidences);
                    }
                    viewItems.CountAllFiles = Interlocked.Increment(ref changeCoutAll);
                }
            }

            catch
            {
                childrenItems.Add(new ModelItem
                {
                    Header = GetFileOrFolderName(path),
                    FullPath = path,
                    Image = GetImage(path),
                    Status = "Not available",
                    VolumeMemory = -1
                });
            }
            return childrenItems;
        }


        //Сюда вставить подсчет кол-ва файлов
        private double GetVolMem(string path, ref double catalogSize)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] diA = di.GetDirectories();
                FileInfo[] fi = di.GetFiles();
                // Cycle for all files
                foreach (FileInfo f in fi)
                {
                    // length Bytes
                    catalogSize += f.Length;
                }
                // Cycle for directory
                foreach (DirectoryInfo df in diA)
                {
                    GetVolMem(df.FullName, ref catalogSize);
                }
                //1ГБ = 1024 Байта * 1024 КБайта * 1024 МБайта
                return catalogSize;
            }
            catch
            {
                return 0;
            }
        }

        private string GetFileOrFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return (string.Empty);

            string normalPath = path.Replace('/', '\\');
            int lastIndexName = normalPath.LastIndexOf('\\');

            if (lastIndexName <= 0)
                return path;

            //Take last value in string-path with separated \\
            return path.Substring(lastIndexName + 1);
        }

        private BitmapSource GetImage(string path)
        {
            //Get a full path
            if (path == null)
                return null;

            string image = "Images/file.png";
            string nameAFile = GetFileOrFolderName(path);

            //If name == null => drive
            if (string.IsNullOrEmpty(nameAFile))
                image = "Images/drive.png";
            else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
                image = "Images/folder-closed.png";

            var _image = new BitmapImage(new Uri($"pack://application:,,,/{image}"));
            _image.Freeze();

            return _image;
        }
    }
}
