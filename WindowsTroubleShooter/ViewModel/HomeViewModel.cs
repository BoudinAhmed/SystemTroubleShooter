using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using WindowsTroubleShooter.Model;



namespace WindowsTroubleShooter.ViewModel
{
    //- Will add IDisposable if using timers or event listeners that need cleanup
    public class HomeViewModel : ViewModelBase
    {

        //Declaration
        private string _version;
        private IssueItemViewModel _lastClickedItem;
        private Border _lastClickedBorder;
        private ObservableCollection<IssueItemViewModel> _issueItems; // This holds the full, original list

        // New fields for Search and Filtering
        private string _searchQuery;
        private ICollectionView _filteredIssueItems; // The view that the UI binds to


        // --- Public Properties Binding --- //

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        // This property holds the original, full list of issues.
        // The UI's ItemsControl will now bind to FilteredIssueItems instead.
        public ObservableCollection<IssueItemViewModel> IssueItems
        {
            get => _issueItems;
            private set => SetProperty(ref _issueItems, value);
        }

        // This is the property the search TextBox will bind to
        public string SearchQuery
        {
             get => _searchQuery;
             set
             {
                 if (SetProperty(ref _searchQuery, value))
                 {
                     // Refresh the filtered view whenever the search query changes
                     FilteredIssueItems.Refresh();
                 }
             }
         }

         // This is the property the ItemsControl will bind to
         public ICollectionView FilteredIssueItems
         {
             get => _filteredIssueItems;
             private set => SetProperty(ref _filteredIssueItems, value);
         }


        public HomeViewModel()
        {
            _issueItems = new ObservableCollection<IssueItemViewModel>();

            LoadVersionInfo();
            LoadIssueItems(); // This populates the _issueItems collection

            //- // Initialize the ICollectionView after loading the initial data
            _filteredIssueItems = CollectionViewSource.GetDefaultView(IssueItems);

            //- // Assign the filter method
             if (_filteredIssueItems != null) // Ensure the view was created successfully
             {
                 _filteredIssueItems.Filter = FilterIssues;
             }


            // TODO: Implenment timer or event listener call UpdateSystemStatus() in a span time

            // _lastClickedItem and _lastClickedBorder are present but not used in the provided code
        }

        private void LoadIssueItems()
        {
            //Hardcoded for now will figure something out
            // Populate the IssueItems collection with all available issues
            IssueItems.Add(new IssueItemViewModel { Title = "Internet Connection", Description = "Fix problems with connecting to the internet", ImageSource = "\xE701", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Windows Update", Description = "Resolve problem with windows update", ImageSource = "\xE895", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Sound", Description = "Fix problems with playing audio", ImageSource = "\xE767", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Map Network Drive", Description = "Map a network drive with letter and path", ImageSource = "\xE8CE", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Display Settings", Description = "Adjust screen resolution and scaling", ImageSource = "\xE7F4", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Bluetooth Devices", Description = "Pair or troubleshoot Bluetooth", ImageSource = "\xE702", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Printer Issues", Description = "Resolve problems with printers", ImageSource = "\xE74F", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "System Performance", Description = "Improve speed and responsiveness", ImageSource = "\xE8FE", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "File Explorer", Description = "Fix issues with file management", ImageSource = "\xE8B3", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Network Adapter", Description = "Troubleshoot network adapters", ImageSource = "\xE770", IssueType = new InternetTroubleshooter() });

            // Add more issues as needed
        }

        private void LoadVersionInfo()
        {
            // Hardcoded for now but will load it from assembly info
            Version = "Version 2.0.1";
        }

        //- // Filter method for ICollectionView
         private bool FilterIssues(object item)
         {
             // If search query is empty or whitespace, show all items
             if (string.IsNullOrWhiteSpace(SearchQuery))
             {
                 return true;
             }

             // If the item is an IssueItemViewModel, check if its Title contains the search query
             if (item is IssueItemViewModel issue)
             {
                 // Perform case-insensitive search on the Issue Title
                 return issue.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0;
             }

        //-     // Hide items that are not IssueItemViewModel (shouldn't happen in this case)
             return false;
             }

        // Methods related to clicking issue items (_lastClickedItem, _lastClickedBorder) were not provided, keeping the fields as they were in your original code structure.

        // If implementing IDisposable for cleanup (e.g., timers)
         private bool disposed = false;
         protected virtual void Dispose(bool disposing)
         {
             if (!disposed)
             {
                 if (disposing)
                 {
                     // Dispose of any managed resources here (like timers)
                 }
                 // Dispose of any unmanaged resources here
                 disposed = true;
             }
         }

         public void Dispose()
         {
             Dispose(true);
             GC.SuppressFinalize(this);
         }

    }
}