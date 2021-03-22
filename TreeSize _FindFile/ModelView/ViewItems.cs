using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TreeSize;

namespace TreeSize__FindFile.ModelView
{
    public class ViewItems : INotifyPropertyChanged
    {
        protected ObservableCollection<ModelItem> _modelItems;

        public ViewItems()
        {
            _modelItems = new ObservableCollection<ModelItem>();
        }

        public ObservableCollection<ModelItem> ViewItem
        {
            get { return _modelItems; }
        }

        public void Update()
        {
            OnPropertyChanged("ViewItems");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public int Count()
        {
            return ViewItem.Count;
        }

        delegate void addItem(ModelItem item);
        public void AddItem(ModelItem item)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                ViewItem.Add(item);
            else
            {
                addItem add = AddItem;
                Application.Current.Dispatcher.BeginInvoke(add, item);
            }
        }

        delegate void changeItem(ConcurrentStack<ViewItems> stack, ModelItem item);
        public void ChangeItem(ConcurrentStack<ViewItems> stack, ModelItem item)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                ViewItems number;
                stack.TryPop(out number);
                ViewItem.Remove(number.ViewItem[number.Count() - 1]);
                ViewItem.Add(item);
            }

            else
            {
                changeItem Change = ChangeItem;
                Application.Current.Dispatcher.BeginInvoke(Change, stack, item);
            }
        }

        delegate void delete(ConcurrentStack<ViewItems> stack, ModelItem item);
        public void Delete(ConcurrentStack<ViewItems> stack, ModelItem item)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                ViewItems number;
                stack.TryPop(out number);
                ViewItem.Remove(number.ViewItem[number.Count() - 1]);
            }

            else
            {
                delete _delete = Delete;
                Application.Current.Dispatcher.BeginInvoke(_delete, stack, item);
            }
        }


        delegate void inkrement(ConcurrentStack<ViewItems> stack, bool flag);
        public void Inkrement(ConcurrentStack<ViewItems> stack, bool flag = false)
        {
            if (flag)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    stack.TryPop(out ViewItems number);
                    CountAllFiles++;
                    CountCoincidencesFiles++;
                }

                else
                {
                    inkrement _inkrement = Inkrement;
                    Application.Current.Dispatcher.BeginInvoke(_inkrement, stack, flag);
                }
            }

            else
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    stack.TryPop(out ViewItems number);
                    CountAllFiles++;
                }

                else
                {
                    inkrement _inkrement = Inkrement;
                    Application.Current.Dispatcher.BeginInvoke(_inkrement, stack, flag);
                }
            }
        }

        protected int countAllFiles;
        public int CountAllFiles
        {
            get { return countAllFiles; }
            set
            {
                countAllFiles = value;
                OnPropertyChanged("CountAllFiles");
            }
        }

        protected int countCoincidencesFiles;
        public int CountCoincidencesFiles
        {
            get { return countCoincidencesFiles; }
            set
            {
                countCoincidencesFiles = value;
                OnPropertyChanged("CountCoincidencesFiles");
            }
        }

        protected string timeCount;
        public string TimeCount
        {
            get { return timeCount; }
            set
            {
                timeCount = value;
                OnPropertyChanged("TimeCount");
            }
        }
    }
}
