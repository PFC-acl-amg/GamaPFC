using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Gama.Common.CustomControls
{
    public enum SearchMode
    {
        Delayed,
        Instant,
    }

    public class SearchBox : TextBox
    {
        private Popup _Popup;
        private ListBox _ListBox;
        private string _TextCache = "";
        private bool _SuppressEvent = false;
        private static int _TimeDelay = 500;
        private DispatcherTimer _SearchEventDelayTimer;

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SearchBox),
                new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        public SearchBox() : base()
        {
            _SearchEventDelayTimer = new DispatcherTimer();
            _SearchEventDelayTimer.Interval = SearchEventTimeDelay.TimeSpan;
            _SearchEventDelayTimer.Tick += new EventHandler(OnSeachEventDelayTimerTick);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var iconBorder = GetTemplateChild("PART_SearchIconBorder") as Border;
            if (iconBorder != null)
            {
                iconBorder.MouseLeftButtonDown += new MouseButtonEventHandler(IconBorder_MouseLeftButtonDown);
                iconBorder.MouseLeftButtonUp += new MouseButtonEventHandler(IconBorder_MouseLeftButtonUp);
                iconBorder.MouseLeave += new MouseEventHandler(IconBorder_MouseLeave);
            }

            _Popup = Template.FindName("PART_Popup", this) as Popup;

            _ListBox = Template.FindName("PART_ListBox", this) as ListBox;
            if (_ListBox != null)
            {
                _ListBox.PreviewMouseDown += new MouseButtonEventHandler(ListBox_MouseUp);
                _ListBox.KeyDown += new KeyEventHandler(ListBox_KeyDown);

                OnItemsSourceChanged(ItemsSource);
                OnItemTemplateChanged(ItemTemplate);
                OnItemContainerStyleChanged(ItemContainerStyle);
                OnItemTemplateSelectorChanged(ItemTemplateSelector);
            }
        }

        public LookupItem SelectedItem
        {
            get { return (LookupItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public int MaxCompletions
        {
            get { return (int)GetValue(MaxCompletionsProperty); }
            set { SetValue(MaxCompletionsProperty, value); }
        }

        public Duration SearchEventTimeDelay
        {
            get { return (Duration)GetValue(SearchEventTimeDelayProperty); }
            set { SetValue(SearchEventTimeDelayProperty, value); }
        }

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public Brush LabelTextColor
        {
            get { return (Brush)GetValue(LabelTextColorProperty); }
            set { SetValue(LabelTextColorProperty, value); }
        }

        public SearchMode SearchMode
        {
            get { return (SearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            private set { SetValue(HasTextPropertyKey, value); }
        }

        public bool IsMouseLeftButtonDown
        {
            get { return (bool)GetValue(IsMouseLeftButtonDownProperty); }
            private set { SetValue(IsMouseLeftButtonDownPropertyKey, value); }
        }

        public event RoutedEventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        public event RoutedEventHandler SelectResult
        {
            add { AddHandler(SelectResultEvent, value); }
            remove { RemoveHandler(SelectResultEvent, value); }
        }

        private void RaiseSearchEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);
        }

        private void RaiseSelectResultEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectResultEvent);
            RaiseEvent(args);
        }

        public void ShowPopup()
        {
            if (_ListBox == null || _Popup == null) InternalClosePopup();
            else if (_ListBox.Items.Count == 0) InternalClosePopup();
            else InternalOpenPopup();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            SelectAll();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            var fs = FocusManager.GetFocusScope(this);
            var o = FocusManager.GetFocusedElement(fs);
            if (e.Key == Key.Escape)
            {
                InternalClosePopup();
                Focus();
            }
            else if (e.Key == Key.Down)
            {
                if (_ListBox != null && o == this)
                {
                    _SuppressEvent = true;
                    _ListBox.Focus();
                    _SuppressEvent = false;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.Key == Key.Return || e.Key == Key.Enter)
                && this._Popup.IsOpen)
            {
                SelectedItem = _ListBox.Items[0] as LookupItem;
                RaiseSelectResultEvent();
                InternalClosePopup();
            }

            if (e.Key == Key.Escape)
                Text = "";
            else if (e.Key == Key.Return || e.Key == Key.Enter)
                RaiseSearchEvent();
            else
                base.OnKeyDown(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (_SuppressEvent) return;
            if (_Popup != null)
            {
                InternalClosePopup();
            }
        }

        private void SetTextValueBySelection(bool moveFocus)
        {
            if (_Popup != null)
            {
                InternalClosePopup();
                Dispatcher.Invoke(new Action(() => {
                    Focus();
                    if (moveFocus)
                        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }), System.Windows.Threading.DispatcherPriority.Background);
            }

            _SuppressEvent = true;
            //Text = selectedItem.DisplayMember1;
            SelectAll();
            _SuppressEvent = false;
        }

        private void InternalClosePopup()
        {
            if (_Popup != null)
                _Popup.IsOpen = false;
        }

        private void InternalOpenPopup()
        {
            if (_Popup != null && !_Popup.IsOpen)
            {
                _Popup.IsOpen = true;
                _ListBox.SelectedIndex = -1;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            HasText = Text.Trim().Length != 0;
            _SearchEventDelayTimer.Stop();

            if (e.Changes.First().AddedLength == Text.Length)
                _SuppressEvent = true;

            if (SearchMode == SearchMode.Delayed && !_SuppressEvent && HasText)
            {
                _SearchEventDelayTimer.Stop();
                _SearchEventDelayTimer.Start();
            }
            _SuppressEvent = false;

            if (!HasText)
                InternalClosePopup();
        }

        private void IconBorder_MouseLeftButtonDown(object obj, MouseButtonEventArgs e)
        {
            IsMouseLeftButtonDown = true;
        }

        private void IconBorder_MouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            if (!IsMouseLeftButtonDown) return;

            if (HasText)
                RaiseSearchEvent();

            IsMouseLeftButtonDown = false;
        }

        private void IconBorder_MouseLeave(object obj, MouseEventArgs e)
        {
            IsMouseLeftButtonDown = false;
        }

        void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _SuppressEvent = true;
            SetTextValueBySelection(moveFocus: false);
            SelectedItem = _ListBox.SelectedItem as LookupItem;
            RaiseSelectResultEvent();
            _SuppressEvent = false;
        }

        void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            _SuppressEvent = true;
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SetTextValueBySelection(moveFocus: false);
                SelectedItem = _ListBox.SelectedItem as LookupItem;
                RaiseSelectResultEvent();
            }
            else if (e.Key == Key.Tab)
            {
                SetTextValueBySelection(moveFocus: true);
                SelectedItem = _ListBox.SelectedItem as LookupItem;
                RaiseSelectResultEvent();
            }
            _SuppressEvent = false;
        }

        void OnSeachEventDelayTimerTick(object o, EventArgs e)
        {
            _SearchEventDelayTimer.Stop();
            if (_SuppressEvent) return;

            RaiseSearchEvent();
            // La comprobación de Items.Count previene de una excepción
            // cuya raíz no he sido capaz de identificar. De todas formas,
            // téngase en cuenta que se trata de abrir el popup antes de 
            // lanzar el evento 
            if (_Popup != null && _ListBox != null && _ListBox.Items.Count == 1)
            {
                InternalOpenPopup();
            }

            _TextCache = Text ?? "";

            if (_Popup != null && _TextCache == "")
            {
                InternalClosePopup();
            }
            else if (_ListBox != null)
            {
                if (_Popup != null)
                {
                    if (_ListBox.Items.Count == 0)
                        InternalClosePopup();
                    else
                        InternalOpenPopup();
                }
            }
        }

        #region Dependency Properties

        public static DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
                "LabelText",
                typeof(string),
                typeof(SearchBox));

        public static DependencyProperty LabelTextColorProperty =
            DependencyProperty.Register(
                "LabelTextColor",
                typeof(Brush),
                typeof(SearchBox));

        public static DependencyProperty SearchModeProperty =
            DependencyProperty.Register(
                "SearchMode",
                typeof(SearchMode),
                typeof(SearchBox),
                new PropertyMetadata(SearchMode.Delayed));

        private static DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "HasText",
                typeof(bool),
                typeof(SearchBox),
                new PropertyMetadata());

        public static DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        private static DependencyPropertyKey IsMouseLeftButtonDownPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsMouseLeftButtonDown",
                typeof(bool),
                typeof(SearchBox),
                new PropertyMetadata());

        public static DependencyProperty IsMouseLeftButtonDownProperty =
            IsMouseLeftButtonDownPropertyKey.DependencyProperty;

        public static DependencyProperty SearchEventTimeDelayProperty =
            DependencyProperty.Register(
                "SearchEventTimeDelay",
                typeof(Duration),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(
                    new Duration(new TimeSpan(0, 0, 0, 0, _TimeDelay)),
                    new PropertyChangedCallback(OnSearchEventTimeDelayChanged)));

        public static readonly DependencyProperty MaxCompletionsProperty =
            DependencyProperty.Register(
                "MaxCompletions",
                typeof(int),
                typeof(SearchBox),
                new UIPropertyMetadata(int.MaxValue));

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            ItemsControl.ItemTemplateSelectorProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemTemplateSelectorChanged));

        private static void OnItemTemplateSelectorChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemTemplateSelectorChanged(e.NewValue as DataTemplateSelector);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
                typeof(LookupItem),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemTemplateChanged));

        private static void OnItemTemplateChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemTemplateChanged(e.NewValue as DataTemplate);
        }

        private void OnItemTemplateChanged(DataTemplate p)
        {
            if (_ListBox == null) return;
            _ListBox.ItemTemplate = p;
        }

        public static readonly DependencyProperty ItemContainerStyleProperty =
            ItemsControl.ItemContainerStyleProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemContainerStyleChanged));

        private static void OnItemContainerStyleChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemContainerStyleChanged(e.NewValue as Style);
        }

        private void OnItemContainerStyleChanged(Style p)
        {
            if (_ListBox == null) return;
            _ListBox.ItemContainerStyle = p;
        }

        static void OnSearchEventTimeDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var stb = o as SearchBox;
            if (stb != null)
            {
                stb._SearchEventDelayTimer.Interval = ((Duration)e.NewValue).TimeSpan;
                stb._SearchEventDelayTimer.Stop();
            }
        }

        private void OnItemTemplateSelectorChanged(DataTemplateSelector p)
        {
            if (_ListBox == null) return;
            _ListBox.ItemTemplateSelector = p;
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            ItemsControl.ItemsSourceProperty.AddOwner(
                typeof(SearchBox),
                new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox actb = d as SearchBox;
            if (actb == null) return;
            actb.OnItemsSourceChanged(e.NewValue as IEnumerable);
        }

        protected void OnItemsSourceChanged(IEnumerable itemsSource)
        {
            if (_ListBox == null)
                return;

            if (itemsSource is ListCollectionView)
                _ListBox.ItemsSource = new LimitedListCollectionView((IList)((ListCollectionView)itemsSource).SourceCollection);
            else if (itemsSource is CollectionView)
                _ListBox.ItemsSource = new LimitedListCollectionView(((CollectionView)itemsSource).SourceCollection);
            else if (itemsSource is IList)
                _ListBox.ItemsSource = new LimitedListCollectionView((IList)itemsSource);
            else
            {
                if (itemsSource == null)
                    itemsSource = new List<string>();

                _ListBox.ItemsSource = new LimitedCollectionView(itemsSource);
            }

            if (_ListBox.Items.Count == 0)
                InternalClosePopup();
        }

        #endregion

        public static readonly RoutedEvent SearchEvent =
            EventManager.RegisterRoutedEvent(
                "Search",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(SearchBox));

        public static readonly RoutedEvent SelectResultEvent =
            EventManager.RegisterRoutedEvent(
                "SelectResult",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(SearchBox));
    }
}
