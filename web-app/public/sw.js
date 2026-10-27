/** Service worker of Olevin: shows the push on the snapshot day and opens the snapshot form on a tap. */

const TEXTS = {
	en: {
		title: "Time for your snapshot",
		body: "Today is your snapshot day. Add this month's totals to update your financial calm index.",
	},
	uk: {
		title: "Час для зрізу",
		body: "Сьогодні день зрізу. Внесіть підсумки місяця, щоб оновити індекс фінансового спокою.",
	},
};

const readPayload = (event) => {
	try {
		return event.data?.json() ?? {};
	} catch {
		return {};
	}
};

self.addEventListener("install", () => self.skipWaiting());

self.addEventListener("activate", (event) =>
	event.waitUntil(self.clients.claim()),
);

self.addEventListener("push", (event) => {
	const payload = readPayload(event);
	const language = payload.locale === "UK" ? "uk" : "en";
	const texts = TEXTS[language];

	event.waitUntil(
		self.registration.showNotification(texts.title, {
			body: texts.body,
			icon: "/icons/icon-192.png",
			lang: language,
			tag: "snapshot-day",
			data: { url: `/${language}/snapshots/new` },
		}),
	);
});

self.addEventListener("notificationclick", (event) => {
	event.notification.close();

	const url = new URL(event.notification.data?.url ?? "/", self.location.origin)
		.href;

	event.waitUntil(
		self.clients
			.matchAll({ type: "window", includeUncontrolled: true })
			.then((windows) => {
				const open = windows.find(
					(client) => new URL(client.url).origin === self.location.origin,
				);

				return open
					? open.focus().then((client) => client.navigate(url))
					: self.clients.openWindow(url);
			}),
	);
});
