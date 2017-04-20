#region
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace DesignX.Classes
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private List<CardStackView.Item> _items = new List<CardStackView.Item>();
        private readonly List<Project> _originalList = new List<Project>();
        private readonly List<Project> _projects = new List<Project>();
        public MainPageViewModel()
        {
            _originalList = AzureResult.AzureResults;
            var index = 0;
            var r = new Random();
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
            var files = Directory.GetFiles(documentsPath);
            while (_originalList.Count > 0)
            {
                var i = r.Next(_originalList.Count);
                _projects.Add(_originalList[i]);
                _originalList.RemoveAt(i);
            }
            foreach (var project in _projects)
                if (project.Url != "")
                {
                    foreach (var pathString in files)
                    {
                        if (pathString.Contains(project.Id))
                        {
                            project.Url = pathString;

                        }
                        
                    }
                    _items.Add(new CardStackView.Item
                    {

                        Url = project.Url,
                        Name = project.Name
                    });
                    InformationClass.Dict.Add(index, project.Id);
                    index++;
                }
        }

        public List<CardStackView.Item> ItemsList
        {
            get => _items;
            set
            {
                if (_items == value)
                    return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}