import SignIn from "@features/sign-in";

interface SignInPageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the sign-in page. */
const SignInPage = async ({ params }: Readonly<SignInPageProps>) => {
	const { lang } = await params;

	return <SignIn language={lang} />;
};

export default SignInPage;
