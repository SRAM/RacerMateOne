<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:clitype="clitype" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:java="java" xmlns:link="http://www.xbrl.org/2003/linkbase" xmlns:xbrldi="http://xbrl.org/2006/xbrldi" xmlns:xbrli="http://www.xbrl.org/2003/instance" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <xsl:output version="4.0" method="html" indent="no" encoding="UTF-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd"/>
  <xsl:param name="SV_OutputFormat" select="'HTML'"/>
  <xsl:variable name="XML" select="/"/>
  <xsl:template match="/">
    <html>
      <head>
        <style type="text/css">
          body          { font-size:small; font-family:Verdana;
          background: #76a7ca; color: black;
          vertical-align:top; text-align:left;
          }
          h1, h2, h3	  { }
          h1        	  { font-size:x-large; margin-top: 0em; margin-bottom: 1em}
          h2            { font-size:medium; margin-top: 0em; margin-bottom: 0em }
          h3	          { font-size:small; margin-top: 0em; margin-bottom: 0em }
          table         { width:100%; border-width:0; }
          tr            { vertical-align:top;}
          td            { border-top-style:none; border-width: thin; empty-cells:hidden; padding: 5px 5px 15px 2px}
          blockquote    { background: yellow; }
        </style>
      </head>
      <body>
        <xsl:for-each select="$XML">
          <br/>
          <table >
            <tbody>
              <tr >
                <td  style="width:2.0in; ">
                  <img src="..\ReportTemplates\RM1Logo.gif"/>
                </td>
                <td>
                  <img height="98" align="right" src="..\ReportTemplates\RideStudioLogo.jpg" />
                </td>
              </tr>
            </tbody>
          </table>
          <br/>
          <table>
            <tbody>
              <tr>
                <td  style="width:2.0in; ">
                  <h1>
                    <xsl:text>Ride Report</xsl:text>
                  </h1>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Rider:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Rider">
                      <xsl:for-each select="FirstName">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span>
                    <xsl:text>&#160;</xsl:text>
                  </span>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Rider">
                      <xsl:for-each select="LastName">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span>
                    <xsl:text> (</xsl:text>
                  </span>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Rider">
                      <xsl:for-each select="NickName">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span>
                    <xsl:text>)</xsl:text>
                  </span>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Course Mode:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="RideInput">
                      <xsl:for-each select="ModeSettings">
                        <xsl:for-each select="@DisplayString">
                          <span/>
                          <xsl:value-of select="string(.)"/>
                        </xsl:for-each>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Course Ridden:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="RideInput">
                      <xsl:for-each select="Global">
                        <xsl:for-each select="Course">
                          <xsl:apply-templates/>
                        </xsl:for-each>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Date:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="DateOnReport">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Ride Duration:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideDuration">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </td>
              </tr>

              <tr>
                <td>
                  <h3>
                    <xsl:text>Distance:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideDistance">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Speed:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideSpeedAverage">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> (</span>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideSpeedMax">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> max)</span>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Power:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RidePowerAverage">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> (</span>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RidePowerMax">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> max)</span>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Heart Rate:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideHRAverage">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> (</span>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideHRMax">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> max)</span>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Cadence:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideCadenceAverage">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> (</span>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="RideCadenceMax">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                  <span> max)</span>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Saved Performance:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="SavedPerformance">
                        <xsl:for-each select="FileName">
                          <xsl:apply-templates/>
                        </xsl:for-each>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </td>
              </tr>
              <tr>
                <td>
                  <h3>
                    <xsl:text>Exported Performance:</xsl:text>
                  </h3>
                </td>
                <td>
                  <xsl:for-each select="RideOutput">
                    <xsl:for-each select="Results">
                      <xsl:for-each select="ExportedPerformance">
                        <xsl:for-each select="FileName">
                          <xsl:apply-templates/>
                        </xsl:for-each>
                      </xsl:for-each>
                    </xsl:for-each>
                  </xsl:for-each>
                </td>
              </tr>
              <tr>
                <td>
                  <h3 style="color: gray">
                    <xsl:text>Error Codes:</xsl:text>
                  </h3>
                </td>
                <td>
                  <span style="color: gray">
                    <xsl:for-each select="RideOutput">
                      <xsl:for-each select="ExitCodeText">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                    <span>
                      <xsl:text> (#</xsl:text>
                    </span>
                    <xsl:for-each select="RideOutput">
                      <xsl:for-each select="ExitCode">
                        <xsl:apply-templates/>
                      </xsl:for-each>
                    </xsl:for-each>
                    <xsl:text>)</xsl:text>
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
