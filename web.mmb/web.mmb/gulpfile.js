var gulp = require('gulp'),
   jslint = require('gulp-eslint'),
    sass = require('gulp-sass'),
    babel = require('gulp-babel'),
   sassLint = require('gulp-sass-lint');

var config = {
    paths: {
        js: 'widgets/**/*.js',
        scss: 'themes/**/*.scss',
        buildscss: 'themes/*/main.scss'
    }
}

gulp.error = function (err) {
    console.log(err);
    this.emit('end');
};

gulp.task('jslint', function () {
    return gulp.src(config.paths.js)
       .pipe(jslint({ config: 'eslint.config.json' }))
       .pipe(jslint.format())
       .pipe(jslint.failAfterError());
});

gulp.task('scsslint', function () {
    return gulp.src(config.paths.scss)
        .pipe(sassLint())
        .pipe(sassLint.format())
        .pipe(sassLint.failOnError());
});

gulp.task('js-build', function () {
    return gulp.src(config.paths.js)
    .pipe(babel({
        presets: ['es2015']
    }))
        .pipe(gulp.dest('assets/script'));
});

gulp.task('scss', function () {
    return gulp.src(config.paths.buildscss, { base: process.cwd() })
        .pipe(sass().on('error', gulp.error))
        .pipe(gulp.dest('assets/css'));
});

gulp.task('watch', function () {
    gulp.watch(config.paths.scss, ['scsslint', 'scss']);
    gulp.watch(config.paths.js, ['jslint', 'js-build']);
});

gulp.task('default', ['scss', 'js-build']);