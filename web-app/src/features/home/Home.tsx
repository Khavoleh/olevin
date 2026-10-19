import { Button, Card, Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { HOME_I18N } from "./home-i18n";
import { signOut } from "./sign-out";

interface HomeProps {
	language: string;
}

/** Home page of a signed-in user: the sign-out button. */
const Home = ({ language }: Readonly<HomeProps>) => {
	const t = getI18n(language, HOME_I18N);

	return (
		<Card>
			<Heading level={1}>{t("title")}</Heading>

			<form action={signOut} className="mt-6">
				<Button type="submit" variant="secondary" className="w-full">
					{t("sign_out")}
				</Button>
			</form>
		</Card>
	);
};

export default Home;
