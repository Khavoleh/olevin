import type { I18N } from "@shared/interfaces";

/** Texts of the privacy policy page. */
export const PRIVACY_I18N: I18N = {
	en: {
		title: "Privacy policy",
		intro:
			"Olevin turns a monthly snapshot of your finances into a financial calm index. This policy explains what data the app collects, why, where it is kept and how you control it. Olevin is run by Oleh Khavar, an individual living in Alberta, Canada, who is responsible for your data.",

		collect_title: "1. Information we collect",
		collect_intro: "Olevin collects only what it needs to work:",
		collect_account:
			"Account data: your email address and, if you sign up with email, your password, stored only as a hash.",
		collect_google:
			"Google profile: if you sign in with Google, we receive your name, email address and profile picture from Google.",
		collect_settings:
			"Settings: your base currency, interface language, snapshot day and time zone.",
		collect_snapshots:
			"Financial snapshots: the amounts you enter every month — income, expenses by category, savings, liquid reserves and debt payments — and the estimates of earlier months entered during onboarding.",
		collect_research:
			"Questionnaire answers: your answers to the CFPB Financial Well-Being Scale, only if you agreed to the research.",
		collect_push:
			"Push subscriptions: if you turn on notifications, the push address of your browser, its encryption keys and a device name such as “Chrome on macOS”.",
		collect_technical:
			"Technical data: server logs and error reports with the time and path of requests, your browser and operating system, your user ID and error details. Your IP address is processed to deliver the app and protect it from abuse.",
		collect_no_bank:
			"Olevin does not connect to your bank and never asks for account or card numbers. The amounts are entered by you and can be approximate.",

		use_title: "2. How we use your information",
		use_intro: "Your data is used to:",
		use_service:
			"provide the app: calculate your index, its history and trend, and the advice, and show amounts in your base currency;",
		use_reminders:
			"send the snapshot day reminder to the devices where you turned on notifications;",
		use_emails:
			"send account emails, such as an email address confirmation or a password reset;",
		use_security: "keep the app secure and find and fix errors;",
		use_statistics:
			"calculate anonymised aggregate statistics for the author's own publications.",
		use_no_sale:
			"Your data is never sold, used for advertising or shared with anyone for their own purposes.",

		bases_title: "3. Legal bases",
		bases_intro:
			"If you are in the European Economic Area or the United Kingdom, your data is processed on these legal bases under the GDPR:",
		bases_contract:
			"performance of a contract — your account, settings, snapshots, reminders and account emails, to provide the app to you;",
		bases_consent: "consent — the research questionnaire;",
		bases_interests:
			"legitimate interests — security, error monitoring and anonymised aggregate statistics.",

		research_title: "4. Research and aggregate statistics",
		research_consent:
			"During onboarding you can agree that your anonymised questionnaire answers may be used for research. The answers are used only in the author's own research and are never shared with anyone in raw form.",
		research_withdraw:
			"You can withdraw the consent in your profile at any time. Your answers are then deleted and are not used in later analysis. Once withdrawn, the consent cannot be given again in the app.",
		research_aggregate:
			"By accepting the Terms of use, you agree that your snapshots may be used in anonymised aggregate statistics for the author's own publications, such as articles, blog posts or studies — for example the average index of all users or the average change of the index over a year. These statistics are calculated over groups large enough that no one can be singled out, and never include individual amounts, answers or anything else that identifies a user.",

		processors_title: "5. Where data is kept and who processes it",
		processors_intro:
			"The app, its database, the Logto sign-in service, logs and error monitoring run on our own servers near Montreal, Quebec, Canada. Some data is processed by these providers:",
		processors_ovh: "OVHcloud — the hosting provider of the servers (Canada);",
		processors_google:
			"Google — signs you in if you choose Google, under Google's own privacy policy;",
		processors_resend:
			"Resend — delivers account emails and receives your email address and the email content;",
		processors_push:
			"the push service of your browser (for example Google, Mozilla or Apple) — delivers notifications and receives your push address and the notification text;",
		processors_rates:
			"ExchangeRate-API — provides currency exchange rates and receives no data about you.",
		processors_transfers:
			"If you live outside Canada, your data is transferred there. For users in the EU and the UK, transfers to Canada and to providers in other countries rely on safeguards required by the GDPR, such as the adequacy decision for Canada or the EU Standard Contractual Clauses.",

		storage_title: "6. Cookies and browser storage",
		storage_intro:
			"Olevin uses no tracking or advertising cookies. Your browser keeps only what the app needs to work:",
		storage_session: "session cookies of Logto that keep you signed in;",
		storage_language:
			"the “preferred-language” cookie that remembers your language for a year;",
		storage_onboarded:
			"the “onboarded” cookie that remembers that you finished onboarding, for a year;",
		storage_draft:
			"the onboarding draft in the session storage of the tab, so a reload doesn't lose it; it is removed once saved or when the tab is closed;",
		storage_service_worker:
			"a service worker that lets you install Olevin as an app and receive notifications.",

		retention_title: "7. How long data is kept",
		retention_account:
			"Your account, settings and snapshots are kept while your account exists.",
		retention_deletion:
			"When you delete your account, all your data is deleted from the app at once and disappears from backups within 30 days.",
		retention_logs:
			"Server logs, metrics and error reports are kept for 30 days.",
		retention_aggregate:
			"Anonymised aggregate statistics identify no one and may be kept without a time limit.",

		rights_title: "8. Your rights",
		rights_intro: "You control your data right in the app:",
		rights_edit: "change your settings and snapshots at any time;",
		rights_export: "download your snapshots as JSON or CSV in your profile;",
		rights_consent: "withdraw the research consent in your profile;",
		rights_push: "turn off notifications on any device;",
		rights_delete:
			"delete your account and all its data in the account center.",
		rights_email:
			"You can also email me to access, correct, delete or receive a copy of your data, to object to or restrict its processing, or with any question about this policy. You also have the right to lodge a complaint with the Office of the Privacy Commissioner of Canada or, if you are in the EU, with your data protection authority.",

		children_title: "9. Children",
		children_text:
			"Olevin is not intended for people under 16. If you believe a child has created an account, let me know and it will be deleted.",

		changes_title: "10. Changes to this policy",
		changes_text:
			"When this policy changes, the date at the top is updated. Significant changes are announced in advance.",

		contact_title: "11. Contact",
		contact_text:
			"For any question about your data or this policy, email me at",
	},
	uk: {
		title: "Політика конфіденційності",
		intro:
			"Olevin перетворює щомісячний зріз ваших фінансів на індекс фінансового спокою. Ця політика пояснює, які дані збирає застосунок, навіщо, де вони зберігаються і як ви ними керуєте. Olevin веде Олег Хавар — фізична особа, що проживає в Альберті, Канада, і відповідає за ваші дані.",

		collect_title: "1. Які дані ми збираємо",
		collect_intro: "Olevin збирає лише те, що потрібно для роботи:",
		collect_account:
			"Дані акаунта: ваша email-адреса і, якщо ви реєструєтеся через пошту, пароль, який зберігається лише як хеш.",
		collect_google:
			"Профіль Google: якщо ви входите через Google, ми отримуємо від Google ваше ім'я, email-адресу та фото профілю.",
		collect_settings:
			"Налаштування: базова валюта, мова інтерфейсу, день зрізу та часовий пояс.",
		collect_snapshots:
			"Фінансові зрізи: суми, які ви вводите щомісяця, — дохід, витрати за категоріями, заощадження, ліквідні резерви та платежі за боргами, — а також оцінки попередніх місяців, введені під час онбордингу.",
		collect_research:
			"Відповіді анкети: ваші відповіді на шкалу фінансового добробуту CFPB, лише якщо ви погодилися на дослідження.",
		collect_push:
			"Push-підписки: якщо ви вмикаєте сповіщення, — push-адреса вашого браузера, його ключі шифрування та назва пристрою, наприклад «Chrome на macOS».",
		collect_technical:
			"Технічні дані: серверні логи та звіти про помилки з часом і шляхом запитів, браузером і операційною системою, вашим ID користувача та деталями помилки. Ваша IP-адреса обробляється, щоб доставити застосунок і захистити його від зловживань.",
		collect_no_bank:
			"Olevin не підключається до вашого банку і ніколи не просить номерів рахунків чи карток. Суми вводите ви самі, і вони можуть бути приблизними.",

		use_title: "2. Як ми використовуємо дані",
		use_intro: "Ваші дані використовуються, щоб:",
		use_service:
			"надавати застосунок: рахувати ваш індекс, його історію і тренд, поради та показувати суми в базовій валюті;",
		use_reminders:
			"надсилати нагадування в день зрізу на пристрої, де ви ввімкнули сповіщення;",
		use_emails:
			"надсилати листи щодо акаунта, як-от підтвердження email-адреси чи скидання пароля;",
		use_security: "захищати застосунок, знаходити та виправляти помилки;",
		use_statistics:
			"рахувати знеособлену агреговану статистику для власних публікацій автора.",
		use_no_sale:
			"Ваші дані ніколи не продаються, не використовуються для реклами й не передаються нікому для власних цілей.",

		bases_title: "3. Правові підстави",
		bases_intro:
			"Якщо ви перебуваєте в Європейській економічній зоні або Великій Британії, ваші дані обробляються на таких правових підставах за GDPR:",
		bases_contract:
			"виконання договору — акаунт, налаштування, зрізи, нагадування та листи щодо акаунта, щоб надавати вам застосунок;",
		bases_consent: "згода — анкета для дослідження;",
		bases_interests:
			"законні інтереси — безпека, моніторинг помилок і знеособлена агрегована статистика.",

		research_title: "4. Дослідження та агрегована статистика",
		research_consent:
			"Під час онбордингу ви можете погодитися, що ваші знеособлені відповіді анкети можуть використовуватися для досліджень. Відповіді використовуються лише у власному дослідженні автора і ніколи нікому не передаються в сирому вигляді.",
		research_withdraw:
			"Ви можете відкликати згоду в профілі будь-коли. Тоді ваші відповіді видаляються і не використовуються в подальшому аналізі. Після відкликання згоду не можна надати знову в застосунку.",
		research_aggregate:
			"Приймаючи Умови використання, ви погоджуєтеся, що ваші зрізи можуть використовуватися у знеособленій агрегованій статистиці для власних публікацій автора — статей, дописів у блозі чи досліджень, — наприклад середній індекс усіх користувачів чи середня зміна індексу за рік. Ця статистика рахується по групах, достатньо великих, щоб нікого не можна було виокремити, і ніколи не містить індивідуальних сум, відповідей чи іншого, що ідентифікує користувача.",

		processors_title: "5. Де зберігаються дані та хто їх обробляє",
		processors_intro:
			"Застосунок, його база даних, сервіс входу Logto, логи та моніторинг помилок працюють на наших власних серверах поблизу Монреаля, Квебек, Канада. Частину даних обробляють такі постачальники:",
		processors_ovh: "OVHcloud — хостинг-провайдер серверів (Канада);",
		processors_google:
			"Google — виконує вхід, якщо ви обираєте Google, за власною політикою конфіденційності Google;",
		processors_resend:
			"Resend — доставляє листи щодо акаунта й отримує вашу email-адресу та зміст листа;",
		processors_push:
			"push-сервіс вашого браузера (наприклад, Google, Mozilla чи Apple) — доставляє сповіщення й отримує вашу push-адресу та текст сповіщення;",
		processors_rates:
			"ExchangeRate-API — надає курси валют і не отримує жодних даних про вас.",
		processors_transfers:
			"Якщо ви живете не в Канаді, ваші дані передаються туди. Для користувачів з ЄС і Великої Британії передача до Канади та постачальникам в інших країнах спирається на гарантії, яких вимагає GDPR, як-от рішення про адекватність захисту для Канади чи стандартні договірні умови ЄС.",

		storage_title: "6. Cookies та сховище браузера",
		storage_intro:
			"Olevin не використовує відстежувальних чи рекламних cookies. Ваш браузер зберігає лише те, що потрібно для роботи застосунку:",
		storage_session: "сесійні cookies Logto, що тримають вас у системі;",
		storage_language:
			"cookie «preferred-language», що запам'ятовує вашу мову на рік;",
		storage_onboarded:
			"cookie «onboarded», що на рік запам'ятовує, що ви завершили онбординг;",
		storage_draft:
			"чернетку онбордингу в сесійному сховищі вкладки, щоб перезавантаження її не стерло; вона видаляється після збереження або коли вкладку закрито;",
		storage_service_worker:
			"service worker, що дає змогу встановити Olevin як застосунок і отримувати сповіщення.",

		retention_title: "7. Скільки зберігаються дані",
		retention_account:
			"Ваш акаунт, налаштування та зрізи зберігаються, доки існує акаунт.",
		retention_deletion:
			"Коли ви видаляєте акаунт, усі ваші дані одразу видаляються із застосунку і зникають із резервних копій протягом 30 днів.",
		retention_logs:
			"Серверні логи, метрики та звіти про помилки зберігаються 30 днів.",
		retention_aggregate:
			"Знеособлена агрегована статистика нікого не ідентифікує і може зберігатися без обмеження строку.",

		rights_title: "8. Ваші права",
		rights_intro: "Ви керуєте своїми даними просто в застосунку:",
		rights_edit: "змінюйте налаштування та зрізи будь-коли;",
		rights_export: "завантажуйте свої зрізи у JSON або CSV у профілі;",
		rights_consent: "відкликайте згоду на дослідження в профілі;",
		rights_push: "вимикайте сповіщення на будь-якому пристрої;",
		rights_delete: "видаляйте акаунт і всі його дані в центрі акаунта.",
		rights_email:
			"Також ви можете написати мені, щоб отримати доступ до своїх даних, виправити, видалити їх чи отримати копію, заперечити проти обробки чи обмежити її, або з будь-яким питанням щодо цієї політики. Ви також маєте право подати скаргу до Офісу уповноваженого з питань приватності Канади або, якщо ви в ЄС, до свого органу із захисту даних.",

		children_title: "9. Діти",
		children_text:
			"Olevin не призначений для осіб, молодших за 16 років. Якщо ви вважаєте, що акаунт створила дитина, повідомте мене, і його буде видалено.",

		changes_title: "10. Зміни цієї політики",
		changes_text:
			"Коли ця політика змінюється, дата вгорі оновлюється. Про суттєві зміни буде повідомлено заздалегідь.",

		contact_title: "11. Контакти",
		contact_text:
			"З будь-яким питанням щодо ваших даних чи цієї політики пишіть мені на",
	},
};
