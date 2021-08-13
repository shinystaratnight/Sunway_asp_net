<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <xsl:template match="/">

    <!--<header class="kenwood-unique-content">
      <div class="header-middle-container">
        <div class="container nav-container clearfix">
          <div class="HeaderContainer">
            <a href="/" class="logo">
              <img src="templates/kenwood/img/logo.png" alt="Site logo"/>
            </a>
            <div class="mobile-menu-btn handheld"></div>
          </div>
          <div class="phone-and-time">
            020 7749 9220
            <span class="CurrentOpeningTime"></span>
          </div>
          <div class="OpeningTimes" id="opening-times-mobile">
            <ul>
              <li>Monday - Thursday 8:30 - 22:00</li>
              <li>Friday 8:30 - 19:00</li>
              <li>Saturday  9:00 - 21:00</li>
              <li>Sunday  10:00 - 21:00</li>
              <li>Bank Holidays  9:00 - 17:00</li>
            </ul>
          </div>
        </div>
      </div>
      <nav id="nav-bar">
        <div class="container clearfix">
          <div class="phone-wrap f-right">
            <div class="phone-number" id="contact_phone">020 7749 9220</div>
          </div>
        </div>
      </nav>
    </header>-->

    <header class="kenwood-unique-content">
      <div class="header-middle-container">
        <div class="container nav-container clearfix">
          <div class="HeaderContainer">
            <a href="/" class="logo">
              <img src="https://www.kenwoodtravel.co.uk/templates/kenwood/img/logo.png" alt="Site logo"/>
            </a>
            <div class="mobile-menu-btn handheld"></div>
          </div>
          <div class="phone-and-time">
            020 7749 9220
            <span class="CurrentOpeningTime" id="CurrentOpeningTime"></span>
          </div>
          <div class="OpeningTimes" id="opening-times-mobile">
            <ul>
              <li>Monday - Thursday 8:30 - 22:00</li>
              <li>Friday 8:30 - 19:00</li>
              <li>Saturday  9:00 - 21:00</li>
              <li>Sunday  10:00 - 21:00</li>
              <li>Bank Holidays  9:00 - 17:00</li>
            </ul>
          </div>
        </div>
      </div>
      <nav id="nav-bar">
        <div class="container clearfix">
          <ul class="breadcrumbs">
            <li itemscope="" itemtype="http://data-vocabulary.org/Breadcrumb">
              <a href="/" itemprop="url" class="current">
                <span itemprop="title">Home</span>
              </a>
            </li>
          </ul>
          <div class="phone-wrap f-right">
            <div class="phone-number" id="contact_phone">020 7749 9220</div>
          </div>
          <div class="search-wrap f-right">
            <xsl:text> </xsl:text>
          </div>
        </div>
      </nav>

    </header>

    <script type="text/javascript" src="/themes/Kenwood/widgets/header/header.js"></script>
    <script>
      kenwoodHeaderJs();
    </script>
  </xsl:template>

</xsl:stylesheet>
