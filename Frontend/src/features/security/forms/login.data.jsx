import * as Yup from 'yup';

export const loginInitValues = {
    email: '',
    password: '',
}

export const loginValidationSchema = Yup.object ({
    email: Yup.string()
        .required('El correo electrónico es requerido.')
        .email('Ingrese un correo electrónico válido.'),
    password: Yup.string()
        .required('La contraseña es requerida.')
})