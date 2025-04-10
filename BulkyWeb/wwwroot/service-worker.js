
const CACHE_NAME = 'bulky-cache-v1'; // 更改版本号以更新缓存
const OFFLINE_URL = '/offline.html'; // 我们将创建的离线回退页面

// 需要缓存的核心文件列表
const urlsToCache = [
    '/',                      // 首页
    '/offline.html',          // 离线页面
    '/css/site.css',          // 主要样式表
    '/js/site.js',            // 主要脚本
    // --- 核心库 ---
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/lib/bootstrap/dist/js/bootstrap.bundle.min.js',
    '/lib/jquery/dist/jquery.min.js',
    // --- 添加其他重要的静态资源 ---
    '/images/logo.png',
];

// 安装事件 - 缓存核心资源
self.addEventListener('install', event => {
    console.log('[Service Worker] Install');
    self.skipWaiting();

    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('[Service Worker] Caching all: app shell and content');
                return cache.addAll(urlsToCache);
            })
    );
});

// 激活事件 - 清理旧缓存
self.addEventListener('activate', event => {
    console.log('[Service Worker] Activate');
    event.waitUntil(
        caches.keys().then(keyList => {
            return Promise.all(keyList.map(key => {
                if (key !== CACHE_NAME) {
                    console.log('[Service Worker] Removing old cache', key);
                    return caches.delete(key);
                }
            }));
        })
    );
    return self.clients.claim();
});

// 请求拦截 - 实现离线功能
self.addEventListener('fetch', event => {
    console.log('[Service Worker] Fetch', event.request.url);

    event.respondWith(
        caches.match(event.request)
            .then(response => {
                // 缓存命中 - 返回缓存的响应
                if (response) {
                    console.log('[Service Worker] Found in Cache', event.request.url);
                    return response;
                }

                // 复制请求，因为请求只能使用一次
                const fetchRequest = event.request.clone();

                // 尝试网络请求
                return fetch(fetchRequest)
                    .then(response => {
                        // 验证响应是否有效
                        if (!response || response.status !== 200 || response.type !== 'basic') {
                            return response;
                        }

                        // 复制响应，因为响应只能使用一次
                        const responseToCache = response.clone();

                        // 将新响应添加到缓存
                        caches.open(CACHE_NAME)
                            .then(cache => {
                                cache.put(event.request, responseToCache);
                                console.log('[Service Worker] New Data Cached', event.request.url);
                            });

                        return response;
                    })
                    .catch(() => {
                        // 离线时，对于HTML请求返回离线页面
                        if (event.request.headers.get('accept').includes('text/html')) {
                            console.log('[Service Worker] Return offline page instead');
                            return caches.match('/offline.html');
                        }
                    });
            })
    );
});