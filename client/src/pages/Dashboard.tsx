import axios from 'axios';
import { useEffect, useState } from 'react';
import Modal from '../components/Modal';
import { Field, Form, Formik } from 'formik';
import ButtonMain from '../components/ui/ButtonMain';
import { Link } from 'react-router-dom';
import { ProjectData } from '../utils/types';

interface CreateProject {
  name: string;
  description?: string;
}

export default function Dashboard() {
  const [data, setData] = useState<ProjectData[] | undefined>();
  const [open, setOpen] = useState(false);

  useEffect(() => {
    axios
      .get('/api/projects')
      .then((res) => setData(res.data.projects))
      .catch((err) => console.error(err));
  }, [open]);

  const handleCreateProject = (values: CreateProject) => {
    axios
      .post('/api/projects', {
        name: values.name,
        description: values.description,
      })
      .then(() => setOpen(false))
      .catch((err) => console.error(err));
  };

  return (
    <div className="h-screen bg-login bg-cover bg-no-repeat">
      <div className="mx-auto max-w-7xl bg-white">
        <div className="mt-10 flex justify-between">
          <h1 className="text-2xl font-semibold">Your projects</h1>
          <button
            className="h-10 rounded-md bg-[#0065ff] p-2 text-sm font-semibold text-white"
            onClick={() => setOpen(true)}
          >
            Create new project
          </button>
        </div>
        <div className="my-3 w-full border" />

        <div>
          {data ? (
            <div className="flex flex-wrap">
              {data.map((p) => {
                return (
                  <Link
                    to={`/project/${p.id}`}
                    className="m-5 w-full max-w-52 rounded-md border text-center shadow-sm hover:shadow-md hover:shadow-blue-300"
                  >
                    <div className="">
                      Project:<div>{p.name}</div>
                    </div>
                  </Link>
                );
              })}
            </div>
          ) : (
            <div></div>
          )}
        </div>

        <Modal open={open} onClose={() => setOpen(false)}>
          <div className="w-full text-center">
            <div className="mx-auto my-4 w-48">
              <h3 className="text-lg font-semibold text-gray-800">
                Create new project
              </h3>
            </div>

            <Formik
              initialValues={{ name: '', description: '' }}
              onSubmit={(values: CreateProject) => {
                handleCreateProject(values);
              }}
            >
              {({ isSubmitting }) => (
                <Form className="space-y-3">
                  <Field
                    className="h-10 w-full rounded-md border-2 border-[#dfe1e6] p-2"
                    id="name"
                    type="name"
                    name="name"
                    placeholder="Enter project name"
                    required
                  />
                  <Field
                    className="h-20 w-full rounded-md border-2 border-[#dfe1e6] p-2"
                    type="description"
                    name="description"
                    placeholder="Enter description"
                    as="textarea"
                  />

                  <div className="mx-5 flex justify-between">
                    <button className="" onClick={() => setOpen(false)}>
                      Cancel
                    </button>
                    <ButtonMain
                      text="Create"
                      type="submit"
                      disabled={isSubmitting}
                      className="px-5"
                    />
                  </div>
                </Form>
              )}
            </Formik>
          </div>
        </Modal>
      </div>
    </div>
  );
}
