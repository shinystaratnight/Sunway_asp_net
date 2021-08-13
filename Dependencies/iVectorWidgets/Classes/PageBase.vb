Public Class PageBase
	Inherits System.Web.UI.Page


	Public Sub AddWidgetToContainer(ByVal Container As System.Web.UI.Control, ByVal Widget As Intuitive.Web.Widgets.WidgetBase)
		Container.Controls.Add(Widget)
		Widget.WidgetName = Widget.GetType.Name
		Widget.URLPath = ResolveUrl("/widgets/" & Widget.WidgetName)
	End Sub

End Class
