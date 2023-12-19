using System;
using System.Diagnostics;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;

using ATodoList.Models;
using ATodoList.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;

namespace ATodoList.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        /// <summary>
        /// ��Ϣ��ʾ����
        /// </summary>
        private WindowNotificationManager? _manager;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ��Ϣ��ʾ
        /// </summary>
        /// <param name="e"></param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _manager = new WindowNotificationManager(this) {
                MaxItems = 3,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
            };
        }

        /// <summary>
        /// ��ʾ��Ϣ
        /// </summary>
        /// <param name="title">��Ϣ����</param>
        /// <param name="message">��Ϣ����</param>
        /// <param name="type">��Ϣ����</param>
        private void ShowNotification(string? title, string? message, NotificationType type)
        {
            _manager?.Show(new Avalonia.Controls.Notifications.Notification(title, message, type));
        }

        /// <summary>
        /// ��ʾ��Ϣ����Ϣ��������Ϣ����ȷ��
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="type">��Ϣ����</param>
        private void ShowNotification(string? message, NotificationType type) => ShowNotification(type.ToString(), message, type);

        /// <summary>
        /// �������л��¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupItemList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var selectGroupName = ((sender as ListBox)?.SelectedItem as TodoGroupItem)?.Name ?? string.Empty;

            ViewModel!.SelectedGroupName = selectGroupName;
            ViewModel!.ReloadTodoItemsFromCurrentSelectedGroup();
        }

        /// <summary>
        /// �첽���Ƶ�ǰѡ�е������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Async_GroupItemList_ContextMenu_CopyMenuItem_CopyGroupName(object sender, RoutedEventArgs e)
        {
            var selectGroupName = ViewModel!.SelectedGroupName;
            await AsyncSendTextToSystemClipboard(selectGroupName);
            ShowNotification("�������������Ѹ���", NotificationType.Information);
        }

        /// <summary>
        /// �첽���ַ���������ϵͳ������
        /// </summary>
        /// <param name="text">�������Ƶ�����</param>
        /// <returns></returns>
        private async Task AsyncSendTextToSystemClipboard(string text)
        {
            var clipboard = Clipboard;
            if (clipboard is not null) {
                await clipboard.SetTextAsync(text);
            }
        }

        /// <summary>
        /// �첽����ѡ��δ��ɴ�������ı����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Async_CopySelectedYieldTodoItemTitle(object sender, RoutedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("YieldFinishTodoItemListBox");

            if (listBox?.SelectedItem is TodoItem currentTodoItem) {
                await AsyncSendTextToSystemClipboard(currentTodoItem.Title);
                ShowNotification("������������Ѹ���", NotificationType.Information);
            }
        }

        /// <summary>
        /// �Ƴ�ѡ�е�δ��ɴ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelectedYieldTodoItemTitle(object sender, RoutedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("YieldFinishTodoItemListBox");

            bool result = false;

            if (listBox?.SelectedItem is TodoItem currentTodoItem) {
                result = ViewModel!.RemoveTodoItem(currentTodoItem.ObjectId);
            }

            if (result) {
                ShowNotification("ѡ�д���������ɾ��", NotificationType.Success);
            } else {
                ShowNotification("ѡ�д�������δɾ��", NotificationType.Error);
            }
        }

        /// <summary>
        /// �첽����ѡ������ɵĴ�����������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Async_CopySelectedFinishTodoItemTitle(object sender, RoutedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("FinishedTodoItemListBox");

            if (listBox?.SelectedItem is TodoItem currentTodoItem) {
                await AsyncSendTextToSystemClipboard(currentTodoItem.Title);
                ShowNotification("������������Ѹ���", NotificationType.Information);
            }
        }

        /// <summary>
        /// �Ƴ�ѡ������ɵĴ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelectedFinishTodoItemTitle(object sender, RoutedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("FinishedTodoItemListBox");

            bool result = false;

            if (listBox?.SelectedItem is TodoItem currentTodoItem) {
                result = ViewModel!.RemoveTodoItem(currentTodoItem.ObjectId);
            }

            if (result) {
                ShowNotification("ѡ�д���������ɾ��", NotificationType.Success);
            } else {
                ShowNotification("ѡ�д�������δɾ��", NotificationType.Error);
            }
        }

        /// <summary>
        /// ɾ����ǰѡ�е����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupItemList_ContextMenu_RemoveMeunItem_RemoveSelectedGroup(object sender, RoutedEventArgs e)
        {
            if (ViewModel!.RemoveSelectedGroup(ViewModel!.SelectedGroupName)) {
                ShowNotification("ѡ�д�����������ɾ��", NotificationType.Success);
            } else {
                ShowNotification("ѡ�д���������δɾ��", NotificationType.Error);
            }
        }

        /// <summary>
        /// ��������������ı��䶯�¼������ı�����������ʱ�����ύ��ť
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputGroupTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox t)
                return;

            if (!string.IsNullOrWhiteSpace(t.Text)) {
                CommitGroupNameButton.IsEnabled = true;
            } else {
                CommitGroupNameButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// ��Ӵ���������س��ύ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputGroupNameTextBox_OnEnterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter) {
                return;
            }

            var result = InputGroupNameTextBox?.Text?.Trim();
            if (result is null)
                return;

            InputGroupName.Text = string.Empty;
            if (ViewModel!.AddGroup(result)) {
                ShowNotification("������������ӳɹ�", NotificationType.Success);
            } else {
                ShowNotification("�������������ʧ��", NotificationType.Error);
            }
        }

        /// <summary>
        /// ��Ӵ����������ύ��ť�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitGroupNmaeButton_CommitClick(object sender, RoutedEventArgs e)
        {
            var result = InputGroupNameTextBox?.Text?.Trim();
            if (result is null)
                return;

            InputGroupName.Text = string.Empty;
            if (ViewModel!.AddGroup(result)) {
                ShowNotification("������������ӳɹ�", NotificationType.Success);
            } else {
                ShowNotification("�������������ʧ��", NotificationType.Error);
            }
        }

        /// <summary>
        /// ����������������չ���¼�������������������չ��ʱ������ѡ�Ĵ�����������Ϊѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TodoItemListBox_Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is not Expander expander)
                return;

            var listBoxItem = expander.FindAncestorOfType<ListBoxItem>();
            if (listBoxItem is not null) {
                listBoxItem.IsSelected |= expander.IsExpanded;
            }
        }

        /// <summary>
        /// ��Ӵ�������س��ύ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupItemList_ContextMenu_TextBoxKeyDown_EnterSubmit(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox textBox) {
                return;
            }

            //Debug.WriteLine($"{e.Key} {e.KeyModifiers}");
            if (e.Key is not Key.Enter) {
                return;
            }
                
            //Debug.WriteLine("Submit");

            var newName = textBox.Text?.Trim();
            if (string.IsNullOrEmpty(newName) ) {
                ShowNotification("��Ҫ��������", NotificationType.Warning);
                return;
            }

            textBox.Text = string.Empty;

            var parent = this.FindControl<ListBox>("GroupItemList")?.ContextMenu;

            if (parent is null)
                return;

            //Debug.WriteLine("GroupItemList > ContextMenu");

            if (ViewModel!.RenameSelectedGroupName(newName)) {
                ShowNotification("�������������Ѹ���", NotificationType.Success);
            } else {
                ShowNotification("������������δ����", NotificationType.Error);
            }
        }
    
        /// <summary>
        /// �л������������״̬�¼�
        /// </summary>
        /// <param name="sender">���������CheckBox</param>
        /// <param name="e"></param>
        private void TodoGroupItems_SwitchTodoItemFinishStatus(object sender, RoutedEventArgs e)
        {
            // YieldFinishTodoItemListBox
            if (sender is not CheckBox checkBox) {
                return;
            }

            var listBoxItem = checkBox.FindAncestorOfType<ListBoxItem>();
            if (listBoxItem?.DataContext is not TodoItem item) {
                return;
            }
            
            if (ViewModel!.SwitchTodoItemFinishStatue(item)) {
                ShowNotification($"���� [{item.Title}] {(!item.IsFinish ? "�����" : "δ���")}", NotificationType.Information);
            }
        }

        /// <summary>
        /// �ύ����������޸��¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TodoItemList_CommitTodoItemInfo(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            var grid = button.FindAncestorOfType<Grid>();
            if (grid is null)
                return;

            var todoItemListBoxItem = grid.FindAncestorOfType<ListBoxItem>();
            if (todoItemListBoxItem is null) return;

            if (grid.Children[0] is not TextBox todoItemTitleTextBox)
                return;

            if (grid.Children[1] is not CalendarDatePicker todoItemDeadLineCalendarDatePicker)
                return;

            if (grid.Children[2] is not TextBox todoItemDescriptionTextBox)
                return;

            if (todoItemListBoxItem.DataContext is not TodoItem prevItem) {
                return;
            }

            var title = todoItemTitleTextBox.Text?.Trim() ?? string.Empty;

            var deadLineText = todoItemDeadLineCalendarDatePicker.SelectedDate;

            var description = todoItemDescriptionTextBox.Text?.Trim() ?? string.Empty;

            if (ViewModel!.AlterTodoItemInfo(
                    prevItem.ObjectId,
                    title,
                    deadLineText,
                    description,
                    prevItem.IsFinish
                )) {
                ShowNotification("�������", NotificationType.Success);
            } else {
                ShowNotification("����δ����", NotificationType.Error);
            }
        }

        /// <summary>
        /// �س���Ӵ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTodoItemList_InputTextBox_PressEnterCommit(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter)
                return;

            if (sender is not TextBox textBox)
                return;

            var newItemTitle = textBox.Text;
            textBox.Text = string.Empty;

            if (ViewModel!.AddNewTodoItemToCurrentGroup(newItemTitle)) {
                //textBox.BorderBrush = new SolidColorBrush(Colors.Aquamarine);
                ShowNotification("������ӳɹ�", NotificationType.Success);
            } else {
                //textBox.BorderBrush = new SolidColorBrush(Colors.Red);
                ShowNotification("�������ʧ��", NotificationType.Error);
            }
        }

        /// <summary>
        /// TreeViewItem���Իس�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TodoItemList_TreeViewItem_IgnorePressEnter(object sender, KeyEventArgs e) {
            if (e.Key is Key.Enter) {
                e.Handled = true;
            }
        }

        /// <summary>
        /// �ƶ�δ�������������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YieldFinishTodoItemList_ContextMenu_MoveItem_PointerEnter(object sender, PointerEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return;

            menuItem.Items.Clear();
            foreach (var item in ViewModel!.GroupItems) {
                if (item.Name == ViewModel!.SelectedGroupName) {
                    continue;
                }

                var subItem = new MenuItem() {
                    Header = item.Name,
                };
                subItem.Click += (object? sender, RoutedEventArgs e) => {
                    var listBox = this.FindControl<ListBox>("YieldFinishTodoItemListBox");

                    bool result = false;
                    if (listBox?.SelectedItem is TodoItem currentTodoItem && subItem.Header is string groupName) {
                        result = ViewModel!.RemoveTodoItem(currentTodoItem.ObjectId);
                        result = ViewModel!.AddNewTodoItemToGroup(groupName,
                                                                  currentTodoItem.Title,
                                                                  currentTodoItem.DeadLine,
                                                                  currentTodoItem.Description,
                                                                  currentTodoItem.IsFinish);
                    }

                    if (result) {
                        ShowNotification($"ѡ�д��������Ѿ��ƶ��� {subItem.Header}", NotificationType.Success);
                    } else {
                        ShowNotification("ѡ�д��������ƶ�ʧ��", NotificationType.Error);
                    }
                };

                menuItem.Items.Add(subItem);
            }
        }

        /// <summary>
        /// �ƶ����������������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishTodoItemList_ContextMenu_MoveItem_PointerEnter(object sender, PointerEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return;

            menuItem.Items.Clear();
            foreach (var item in ViewModel!.GroupItems) {
                if (item.Name == ViewModel!.SelectedGroupName) {
                    continue;
                }

                var subItem = new MenuItem() {
                    Header = item.Name,
                };
                subItem.Click += (object? sender, RoutedEventArgs e) => {
                    var listBox = this.FindControl<ListBox>("FinishedTodoItemListBox");

                    bool result = false;
                    if (listBox?.SelectedItem is TodoItem currentTodoItem && subItem.Header is string groupName) {
                        result = ViewModel!.RemoveTodoItem(currentTodoItem.ObjectId);
                        result = ViewModel!.AddNewTodoItemToGroup(groupName,
                                                                  currentTodoItem.Title,
                                                                  currentTodoItem.DeadLine,
                                                                  currentTodoItem.Description,
                                                                  currentTodoItem.IsFinish);
                    }

                    if (result) {
                        ShowNotification($"ѡ�д��������Ѿ��ƶ��� {subItem.Header}", NotificationType.Success);
                    } else {
                        ShowNotification("ѡ�д��������ƶ�ʧ��", NotificationType.Error);
                    }
                };

                menuItem.Items.Add(subItem);
            }
        }

        /// <summary>
        /// �����öԻ����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenNewSettingsWindowButtonClick(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new SettingsWindow();
            settingsDialog.ViewModel = new SettingsWindowViewModel();

            (string?, string?) prev = (
                    (Services.DatabaseService.DB as Services.MongoDBHelper)?.ConnectionString?? null,
                    (Services.DatabaseService.DB as Services.MongoDBHelper)?.DatabaseName ?? null
                );
            
            await settingsDialog.ShowDialog<Unit>(this);
        
            if (prev != (
                    (Services.DatabaseService.DB as Services.MongoDBHelper)?.ConnectionString ?? null,
                    (Services.DatabaseService.DB as Services.MongoDBHelper)?.DatabaseName ?? null
                )) {
                ViewModel!.ResetStatus();
            }
        }
    }

    public class TextLineCaseConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool result && parameter is string str) {
                return result ? str : string.Empty;
            }
            return new BindingNotification(new InvalidCastException(),BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class TextColorFinishCaseConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool result) {
                return result ? "Gray" : "WhiteSmoke";
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}