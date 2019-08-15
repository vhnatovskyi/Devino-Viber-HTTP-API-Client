# Devino-Viber-HTTP-API-Client
[Загальні відомості](http://docs.devinotele.com/) про те, що потрібно зробити для використання API.

*У REST-сервіса не передбачений demo-режим.*

### Аутентифікація
```
var client = new ViberClient(userLogin, userPassword, sourceName);
```
### Отримання статусу відбравленого повідомлення

```
StatusResponse status = cliet.GetStatusMessage(messageId);
```
```
StatusResponse status = client.GetStatusMessage(messagesId);
```

### Відправлення повідомленя

```
SendingReplay result = client.SendMessage(message, resendSms);
```
