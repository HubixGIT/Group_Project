import axios from 'axios';
import { Field, Form, Formik } from 'formik';
import { Link, useNavigate } from 'react-router-dom';
import ButtonMain from '../components/ui/ButtonMain';

interface FormValues {
  email: string;
  password: string;
}

export default function Login() {
  const navigate = useNavigate();

  return (
    <div className="flex h-screen items-center justify-center bg-login bg-cover bg-no-repeat">
      <Formik
        initialValues={{ email: '', password: '' } as FormValues}
        onSubmit={(values) => {
          axios
            .post('/api/login', {
              email: values.email,
              password: values.password,
            })
            .then((res) => {
              if (!res.data) return alert('Something went wrong, try again');
              localStorage.setItem('token', res.data);
              return navigate('/dashboard');
            })
            .catch((err) => {
              console.error(err);
              return alert('Login failed, please check your credentials');
            });
        }}
      >
        {({ isSubmitting }) => (
          <Form className="mx-10 flex w-96 flex-col space-y-3 rounded bg-white p-10 shadow-xl">
            <h2 className="mx-auto mb-3 font-semibold">Log in to continue</h2>
            <Field
              className="h-10 rounded-md border-2 border-[#dfe1e6] p-2"
              id="email"
              type="email"
              name="email"
              placeholder="Enter your email"
              required
            />
            <Field
              className="h-10 rounded-md border-2 border-[#dfe1e6] p-2"
              type="password"
              name="password"
              placeholder="Enter your password"
              required
            />
            <ButtonMain text="Submit" type="submit" disabled={isSubmitting} />

            <Link
              to={'/registration'}
              className="mx-auto max-w-32 text-[#6365ff]"
            >
              Create an account
            </Link>
          </Form>
        )}
      </Formik>
    </div>
  );
}
