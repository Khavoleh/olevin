import { Button, Card, Heading, Text } from "@shared/components";
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
			<Heading level={1}>{t("title")}</Heading>
			<Text variant="description" className="mt-2">
				{t("description")}
			</Text>

			<form action={signIn.bind(null, language)} className="mt-6">
				<Button type="submit" className="w-full">
					{t("sign_in")}
				</Button>
			</form>
		</Card>
	);
};

export default SignIn;
