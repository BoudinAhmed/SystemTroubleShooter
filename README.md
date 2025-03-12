# Windows Troubleshooter

A troubleshooting utility built in C# using the **MVVM (Model-View-ViewModel)** design pattern. This application is designed to assist in diagnosing and fixing common issues with network drives, system settings, 
and other typical computer-related problems. The project is structured using the **WPF** framework for the UI, and **Caliburn.Micro** for navigation and ViewModel binding.

---

## 🚀 Features

- **Network Drive Mapping**: Easily troubleshoot network drive issues with step-by-step diagnostics.
- **Modular**: Designed to be extendable for adding more troubleshooting scenarios in the future.
- **MVVM Architecture**: Clean separation of concerns with Model-View-ViewModel pattern.
- **Real-time Status Updates**: Monitor the status of diagnostics in real-time with a dynamic UI.
- **Command Binding**: Full use of data binding, commands, and INotifyPropertyChanged to keep the UI and logic decoupled.
- **Customizable**: Easily add more troubleshooting models and views.

---

## 📸 Screenshots

![Screenshot of Start View](assets/start-view.png)  
*Start View with an option to navigate to troubleshooting*

![Screenshot of Troubleshooting View](assets/troubleshooting-view.png)  
*View displaying the status messages as issues are diagnosed*

---

## 🛠️ Technologies

- **C#**
- **WPF** (Windows Presentation Foundation)
- **MVVM Pattern**
- **Caliburn.Micro** (for navigation and ViewModel binding)
- **GalaSoft MVVM Light** (for RelayCommand and event handling)

---

## 📦 Installation

1. **Clone the repository**:

   ```bash
   git clone https://github.com/yourusername/WindowsTroubleshooter.git
