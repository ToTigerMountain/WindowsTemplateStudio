﻿Imports Windows.Storage
Imports Windows.System
Imports Windows.UI.Xaml.Media.Animation
Imports Windows.UI.Xaml.Navigation

Imports Param_ItemNamespace.Helpers
Imports Param_ItemNamespace.Models
Imports Param_ItemNamespace.Services

Namespace Views
    Public NotInheritable Partial Class ImageGalleryViewDetailPage
        Inherits Page
        Implements System.ComponentModel.INotifyPropertyChanged

        Private _timer As New DispatcherTimer() With {
             .Interval = TimeSpan.FromMilliseconds(500)
        }

        Private _selectedImage As Object

        Private _source As ObservableCollection(Of SampleImage)

        Public Property SelectedImage As Object
            Get
                Return _selectedImage
            End Get
            Set
                [Param_Setter](_selectedImage, value)
                ApplicationData.Current.LocalSettings.SaveString(ImageGalleryViewPage.ImageGalleryViewSelectedIdKey, DirectCast(SelectedImage, SampleImage).ID)
            End Set
        End Property

        Public Property Source As ObservableCollection(Of SampleImage)
            Get
                Return _source
            End Get
            Set
                [Param_Setter](_source, value)
            End Set
        End Property

        Public Sub New()
            ' TODO WTS: Replace this with your actual data
            Source = SampleDataService.GetGallerySampleData()
            InitializeComponent()
        End Sub

        Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
            MyBase.OnNavigatedTo(e)
            Dim sampleImageId = TryCast(e.Parameter, String)
            If Not String.IsNullOrEmpty(sampleImageId) AndAlso e.NavigationMode = NavigationMode.New Then
                SelectedImage = Source.FirstOrDefault(Function(i) i.ID = sampleImageId)
            Else
                Dim selectedImageId = await ApplicationData.Current.LocalSettings.ReadAsync(Of String)(ImageGalleryViewPage.ImageGalleryViewSelectedIdKey)
                If Not String.IsNullOrEmpty(selectedImageId) Then
                    SelectedImage = Source.FirstOrDefault(Function(i) i.ID = selectedImageId)
                End If
            End If
            Dim animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(ImageGalleryViewPage.ImageGalleryViewAnimationOpen)
            animation?.TryStart(previewImage)
            showFlipView.Begin()
        End Sub

        Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
            MyBase.OnNavigatingFrom(e)
            If e.NavigationMode = NavigationMode.Back Then
                previewImage.Visibility = Visibility.Visible
                ConnectedAnimationService.GetForCurrentView()?.PrepareToAnimate(ImageGalleryViewPage.ImageGalleryViewAnimationClose, previewImage)
            End If
        End Sub

        Private Sub OnShowFlipViewCompleted(sender As Object, e As Object)
            flipView.Focus(FocusState.Programmatic)
        End Sub

        Private Sub OnPageKeyDown(sender As Object, e As KeyRoutedEventArgs)
            If e.Key = VirtualKey.Escape AndAlso NavigationService.CanGoBack Then
                NavigationService.GoBack()
                e.Handled = True
            End If
        End Sub
    End Class
End Namespace
