import axios from 'axios';
import { Field, Formik } from 'formik';
import { Form, Link, useNavigate } from 'react-router-dom';
import ButtonMain from '../components/ui/ButtonMain';

interface FormValues {
  fullName: string;
  email: string;
  password: string;
}

export default function Registration() {
  const navigate = useNavigate();

  return (
    <div className="flex h-screen items-center justify-center bg-login bg-cover bg-no-repeat">
      <Formik
        initialValues={{ fullName: '', email: '', password: '' } as FormValues}
        onSubmit={(values) => {
          console.log('here ' + values);
          const body = {
            fullName: values.fullName,
            email: values.email,
            password: values.password,
          };
          axios
            .post('/api/users', body)
            .then(() => navigate('/login'))
            .catch((err) => {
              console.error(err);
              return alert('Something went wrong, try again');
            });
        }}
      >
        {({ isSubmitting }) => (
          <Form className="mx-10 flex w-96 flex-col space-y-3 rounded bg-white p-10 shadow-xl">
            <h2 className="mx-auto mb-3 font-semibold">Create a new account</h2>
            <Field
              className="h-10 rounded-md border-2 border-[#dfe1e6] p-2"
              id="fullName"
              type="fullName"
              name="fullName"
              placeholder="Enter your full name"
              required
            />
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
              to={'/login'}
              className="mx-auto max-w-64 text-center text-[#6365ff]"
            >
              Already have an account? Log in
            </Link>
          </Form>
        )}
      </Formik>
    </div>
  );
}
