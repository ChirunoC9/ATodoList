using ATodoList.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace ATodoList.Views;

public partial class SettingsWindow : ReactiveWindow<SettingsWindowViewModel>
{
    private WindowNotificationManager? _manager;

    public SettingsWindow()
    {
        InitializeComponent();
    }

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
    /// �ύdatabase���ã�û����д������ΪĬ��host: 127.0.0.1:27017��database name: ATodoList
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CommitSetDatabase(object sender, RoutedEventArgs e)
    {
        var host = string.IsNullOrWhiteSpace(MongoDBHostTextBox.Text) ? "127.0.0.1:27017" : MongoDBHostTextBox.Text;
        var db = string.IsNullOrWhiteSpace(DatabaseTextBox.Text) ? "ATodoList" : DatabaseTextBox.Text;

        if (ViewModel!.SwitchDatabase(host, db)) {
            ShowNotification("�ύ�ɹ�", NotificationType.Success);
        } else {
            ShowNotification("�ύʧ��", NotificationType.Error);
        }
    }
}