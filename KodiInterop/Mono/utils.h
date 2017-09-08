#ifndef __KODISHARP_MONOHOST_UTILS_H
#define __KODISHARP_MONOHOST_UTILS_H

#define dprintf(fmt, ...) \
	fprintf(stderr, "[%s]: " fmt, __func__, ##__VA_ARGS__)

char *remove_ext(const char *mystr);
char *my_basename(const char *path);
char *my_dirname(const char *path);

#endif
