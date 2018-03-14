<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wi="http://schemas.microsoft.com/wix/2006/wi"
    xmlns="http://schemas.microsoft.com/wix/2006/wi">

  <!-- version="2.0" -->
  <!-- <xsl:key name="exe-search" match="wi:Component[ends-with(wi:File/@Source, '.exe')]" use="@Id"/> -->
  <xsl:key name="exe-search" match="wi:Component[substring(wi:File/@Source, string-length(wi:File/@Source) - string-length('.exe') +1)='.exe']" use="@Id"/>
  
  <xsl:template match="wi:Component[key('exe-search', @Id)]" />
  <xsl:template match="wi:ComponentRef[key('exe-search', @Id)]" />

  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="@*"/>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>
  
</xsl:stylesheet>