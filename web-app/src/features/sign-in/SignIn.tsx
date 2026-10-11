import { Button, Card } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { signIn } from "./sign-in";
import { SIGN_IN_I18N } from "./sign-in-i18n";

interface SignInProps {
	language: string;
}

/** Sign-in page: a short description and the button that opens Logto. */
const SignIn = ({ language }: Readonly<SignInProps>) => {
	const t = getI18n(language, SIGN_IN_I18N);

	return (
		<Card className="m-auto">
			<h1>{t("title")}</h1>
			<p className="mt-2 text-fg-muted">{t("description")}</p>

			<form action={signIn.bind(null, language)} className="mt-6">
				<Button type="submit" className="w-full">
					{t("sign_in")}
				</Button>
			</form>
		</Card>
	);
};

export default SignIn;
