var Errata=Errata||{};Errata.Validate=function(t){var a=int.f.GetObject("chkAcceptErrata"),r=int.f.GetObject("lblAcceptErrata");void 0!=a&&null!=a&&void 0!=r&&null!=r&&int.f.SetClassIf(r,"error",!a.checked),null!=t&&void 0!=t&&t()};