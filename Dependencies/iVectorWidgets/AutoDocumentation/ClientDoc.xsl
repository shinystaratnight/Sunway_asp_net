<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/Documentation">

		<html>
			<head>
				<title>Widgets</title>

				<style>
					* {font-family:calibri;color:#444;}
					.widget {margin-bottom:20px;}
					table {border:1px solid #444;border-collapse:collapse;margin-bottom:20px;width:100%;}
					table th, table td {text-align:left;border:1px solid #444;padding:10px;width:50%}
				</style>
				
			</head>
			<body>
				<h1>Widgets</h1>

				<h1>Number of widgets missing settings:</h1>
				<h1>
					<xsl:value-of select="MissingSettings"/>
				</h1>
				
				<xsl:for-each select="Widgets/Widget[DeveloperOnly = 'false']">

					<div class="widget">
						
						<h2>
							<xsl:value-of select="Name"/>
						</h2>

						<h3>Settings</h3>

						<table>
							<tr>
								<th>Setting</th>
								<th>Description</th>
							</tr>
							<xsl:for-each select="Settings/Setting[DeveloperOnly = 'false']">
								<tr>
									<td>
										<xsl:value-of select="Title"/>
									</td>
									<td>
										<xsl:value-of select="Description"/>
									</td>
								</tr>
							</xsl:for-each>
						</table>

					</div>
						
				</xsl:for-each>
			</body>
		</html>
				
    </xsl:template>
</xsl:stylesheet>
