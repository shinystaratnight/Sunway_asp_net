Translation guidelines
When "pre translating" XSL templates you will need to follow some new rules. Certain situations can cause problems.


1. Do not nest {} delimiters inside of xsl:value-of statements or xsl:text as the following substitution will happen

This source code (which will work):

	<div ml="test" mlparams="{$variable1}">
		{0}
	</div>

will turn into:

	<div ml="test">
		<xsl:value-of select="$variable1" />
	</div>

This will not work:

	<div ml="test" mlparams="{$variable1}">
		<xsl:text>{0}</xsl:text>
	</div>

This will also not work:

	<div ml="test" mlparams="{$variable1}">
		<xsl:value-of select="'{0} Adults'" />
	</div>


2. Do not use translation date functions directly in the XSL templates. Instead create a new property on the data class.

This will no longer work:

	<div ml="test" mlparams="{$ArrivalDate}~ShortDate">
		{0}
	</div>

Instead add something like this to the .vb file

	 Public Property ArrivalShortDate As String
        Get
            Return Intuitive.Web.Translation.TranslateAndFormatDate(ArrivalDate, "shortdate")
        End Get
        Set(value As String)
        End Set
    End Property

Add display like this:

	<div ml="test" mlparams="{$ArrivalShortDate}">
		{0}
	</div>


3. If a widget calls the XSLTransform function on WidgetBase from an AJAX call, then it MUST have a property on itself
that specifies the widgetname. It will not have access to the page definition.

eg.

	Me.WidgetName = "Basket"


4. All xsl transforms go through a root function on the WidgetBase class so they will all translate as expected when the 
setting is turned on. We do not have this same luxury with .ascx controls. They previously all directly wrote to the 
writer.

There is now a DrawControl function on WidgetBase that .ascx controls should call - this will handle the translations.


5. Unless calling the XSLTransform function on Widgetbase, all ajax requests will still need to manually translate the
response before returning HTML to the browser - as they do currently.