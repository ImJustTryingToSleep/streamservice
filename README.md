Hub - Управляет фазой сигнализации соединения WebRTC, координируя процесс "рукопожатия" между вещателем видео и несколькими зрителями.

Краткое описание:
  1. Стример регистрируется с помощью RegisterBroadcaster.
  2. Зртиели регистрируются с помощью RegisterWatcher.
  3. Стример инициирует предложение SDP с помощью SendOffer.
  4. Зритель отвечает с помощью SendAnswer.
  5. Обмен кандидатами ICE осуществляется с помощью SendCandidate.
  6. Если зритель отключается, запускается OnDisconnectedAsync, уведомляя вещателя.

live.html - Этот код формирует принимающую сторону системы потокового видео на основе WebRTC.

Краткое описание:
  1. Зритель нажимает кнопку Watch, запуская соединение SignalR.
  2. Зритель регистрируется на сервере, чтобы указать, что он хочет получать видеопоток.
  3. Зритель получает предложение SDP от вещателя и отвечает ответом SDP.
  4. Благодвря обмену ICE кандидатами установиется соединение WebRTC.
  5. После подключения видеопоток от стримера отображается в элементе video.
  6. Если зритель покидает или обновляет страницу, соединения закрываются для освобождения ресурсов.

source.html - Этот код выступает в качестве отправляющей стороны системы потоковой передачи видео на основе WebRTC.

Краткое описание:
  1. Стример захватывает медиапоток экрана.
  2. Он регистрируется как вещатель в хабе.
  3. Когда подключается зритель, стример создает RTCPeerConnection для зрителя и обменивается предложениями/ответами SDP.
  4. Обмен кандидатами ICE осуществляется для установления прямого соединения WebRTC между стримером и каждым зрителем.
  5. Затем поток отправляется напрямую каждому зрителю через WebRTC.

Порядок запуска:
  1. Запустить сервер
  2. Запустить source-клиентб выбрать источник захвата видео
  3. Запустить live-клиент
