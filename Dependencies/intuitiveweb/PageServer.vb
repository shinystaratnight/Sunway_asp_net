Public Interface PageServer
    Function GetPageDefinition(ByVal Request As System.Web.HttpRequest) As PageDefinition
    Function GetPageDefinition(PageName As String) As PageDefinition
    Function GetWidgetReference(ByVal WidgetName As String) As Widgets.WidgetBase
End Interface
