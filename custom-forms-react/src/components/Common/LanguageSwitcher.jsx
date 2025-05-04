import Form from 'react-bootstrap/Form';
import { useTranslation } from 'react-i18next';

const LanguageSwitcher = ({ className }) => {
    const { i18n } = useTranslation();

    const changeLanguage = (event) => {
        const lang = event.target.value;
        i18n.changeLanguage(lang);
        localStorage.setItem('i18nextLng', lang); 
    };

    const availableLanguages = Object.keys(i18n.options.resources || {});

    return (
        <Form.Select
            size="sm"
            value={i18n.language.split('-')[0]}
            onChange={changeLanguage}
            className={className}
            style={{ width: 'auto' }}
            aria-label="Select language"
        >
            {availableLanguages.map((lang) => (
                <option key={lang} value={lang}>
                    {lang.toUpperCase()}
                </option>
            ))}
        </Form.Select>
    );
};

export default LanguageSwitcher;