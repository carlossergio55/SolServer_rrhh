self.addEventListener('install', event => {
    console.log('[SW] Instalando...');
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    console.log('[SW] Activado');
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});
