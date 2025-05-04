import React from 'react';
import { Badge, Spinner } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useGetPopularTagsQuery } from '../../../app/api/tagsApi';
import { useTranslation } from 'react-i18next';

const TagCloud = () => {
    const { t } = useTranslation();
    const TAG_COUNT = 10;

    const { data: tags, isLoading, isError, error } = useGetPopularTagsQuery(TAG_COUNT);

    if (isLoading) {
        return <small className="text-muted">{t('common.loading', 'Loading...')} <Spinner animation="border" size="sm" /></small>;
    }

    if (isError) {
        console.error("Error loading popular tags:", error);
        return <small className="text-danger">{t('home.errorLoadingTags', 'Error loading tags.')}</small>;
    }

    if (!tags || tags.length === 0) {
        return <p className="text-muted">{t('home.noTagsFound', 'No tags found.')}</p>;
    }

    const getTagStyle = (tag) => {
        return {
            variant: 'primary',
            textColor: 'white'
        };
    };

    return (
        <div className="d-flex flex-wrap gap-2 align-items-center">
            {tags.map(tag => {
                const { variant, textColor } = getTagStyle(tag);
                return (
                    <Link
                        to={`/search?tag=${encodeURIComponent(tag.name)}`}
                        key={tag.id}
                        className="text-decoration-none"
                        title={`${tag.count || '?'} templates`}
                    >
                        <Badge
                            pill
                            bg={variant}
                            text={textColor}
                            className="px-3 py-2 fs-6 tag-cloud-badge"
                        >
                            {tag.name}
                        </Badge>
                    </Link>
                )
            })}
            <style>{`
                .tag-cloud-badge:hover {
                    opacity: 0.8;
                    box-shadow: 0 0 5px rgba(0,0,0,0.2);
                }
            `}</style>
        </div>
    );
};

export default TagCloud;