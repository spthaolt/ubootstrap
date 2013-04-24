@echo Configuring permissions 

icacls . /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)
icacls app_code /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)RX
icacls app_browsers /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)RX
icacls app_data /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls bin /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls config /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls css /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls data /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls macroScripts /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls masterpages /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls media /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls scripts /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls umbraco /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls usercontrols /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M
icacls xslt /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)M

PAUSE